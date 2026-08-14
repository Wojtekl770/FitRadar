using FitRadar.Data;
using FitRadar.Repositories.Interfaces;
using FitRadar.Services.Exceptions;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs;
using FitRadar.Shared.DTOs.Auth;
using FitRadar.Shared.Models;
using FitRadar.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IEmailSender = FitRadar.Services.Interfaces.IEmailSender;

namespace FitRadar.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly ILogger<UserService> _logger;
        private readonly FitRadarDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JwtSettings _jwtSettings;


        // Simple in-memory rate limiting
        private static readonly Dictionary<string, DateTime> _lastEmailSent = new();
        private static readonly TimeSpan _emailCooldown = TimeSpan.FromMinutes(1);

        public UserService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            IEmailSender emailSender,
            IConfiguration config,
            ILogger<UserService> logger,
            FitRadarDbContext context,
            IHttpClientFactory httpClientFactory,
            JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _jwtSettings = jwtSettings;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                throw new ArgumentException("Invalid email format.");

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("Last name is required.");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
                throw new EmailAlreadyExistsException();
            }

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = false,
            };

            // Create user with password (Identity handles hashing)
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            // Add to default role
            await _userManager.AddToRoleAsync(user, Roles.User);

            // Generate and send verification code
            await GenerateAndSendVerificationCodeAsync(user);

            _logger.LogInformation("User registered successfully: {Email}", user.Email);
        }

        private async Task SendVerificationEmailAsync(User user, string code)
        {
            var expiryTime = DateTime.UtcNow.AddHours(1).ToLocalTime();

            var subject = "Verify your email address at AlgoRhythm";

            var plain = $@"Hello {user.FirstName}!

            Thank you for registering at FitRadar.

            To complete the registration process, please verify your email address by entering the verification code below:

            {code}

            The code is valid for 1 hour (until {expiryTime:HH:mm, dd.MM.yyyy}).

            If you did not create this account, please ignore this message.

            Best regards,
            FitRadar Team

            ---
            This is an automated message, please do not reply.";


            var html = $@"<p>Hello <strong>{user.FirstName}</strong>!</p>

            <p>Thank you for registering at FitRadar.</p>

            <p>To complete the registration process, please verify your email address by entering the verification code below:</p>

            <p style='font-size: 24px; font-weight: bold; letter-spacing: 2px;'>{code}</p>

            <p>The code is valid for 1 hour (until {expiryTime:HH:mm, dd.MM.yyyy}).</p>

            <p>If you did not create this account, please ignore this message.</p>

            <p>Best regards,<br>FitRadar Team</p>

            <hr>
            <p style='font-size: 12px; color: #666;'>This is an automated message, please do not reply.</p>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email!, subject,plain, html);
                _logger.LogInformation("Verification email sent to: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to: {Email}", user.Email);
                throw;
            }
        }

        private async Task GenerateAndSendVerificationCodeAsync(User user)
        {
            // Generate 6-digit verification code
            var code = Random.Shared.Next(100000, 999999).ToString();
            user.SecurityStamp = code; // Temporary storage
            await _userManager.UpdateAsync(user);

            // Send email
            await SendVerificationEmailAsync(user, code);

            // Update rate limiting
            _lastEmailSent[user.Email!] = DateTime.UtcNow;
        }

        public async Task<AuthResponse> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Email verification attempt for non-existent user: {Email}", request.Email);
                throw new UserNotFoundException();
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already verified for user: {Email}", user.Email);
                return await GenerateAuthResponseAsync(user, "unknown");
            }

            // Verify code (stored in SecurityStamp temporarily)
            if (user.SecurityStamp != request.Code)
            {
                _logger.LogWarning("Invalid verification code for user: {Email}", user.Email);
                throw new InvalidVerificationCodeException();
            }

            // Confirm email
            user.EmailConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString(); // Reset security stamp
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Email verified successfully for user: {Email}", user.Email);

            return await GenerateAuthResponseAsync(user, "unknown");
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string ipAddress)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // Create JWT Access Token
            var key = _jwtSettings.SecretKey;
            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var minutes = _jwtSettings.AccessTokenExpirationMinutes;

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("security_stamp", user.SecurityStamp ?? string.Empty)
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(minutes);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Generate Refresh Token
            var refreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress);

            var userDto = MapToUserDto(user);

            return new AuthResponse(
                tokenString,
                expires,
                userDto,
                refreshToken.Token,
                refreshToken.ExpiresAt
            );
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = GenerateSecureRandomToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Long-lived refresh token
                CreatedByIp = ipAddress
            };

            _context.Set<RefreshToken>().Add(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token generated for user: {UserId}", userId);

            return refreshToken;
        }

        private static string GenerateSecureRandomToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
                throw new UserNotFoundException();
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login attempt with invalid password for user: {Email}", user.Email);
                throw new InvalidPasswordException();
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login attempt with unverified email: {Email}", user.Email);
                throw new EmailNotVerifiedException();
            }

            _logger.LogInformation("User logged in successfully: {Email}", user.Email);

            return await GenerateAuthResponseAsync(user, "unknown");
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            var token = await _context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || !token.IsActive)
            {
                _logger.LogWarning("Refresh token is invalid or expired");
                throw new InvalidRefreshTokenException();
            }

            // Generate new refresh token and revoke old one
            var newRefreshToken = await RotateRefreshTokenAsync(token, ipAddress);

            // Generate new access token
            var user = token.User;
            var roles = await _userManager.GetRolesAsync(user);

            var key = _jwtSettings.SecretKey;
            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var minutes = _jwtSettings.AccessTokenExpirationMinutes;

            var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email!),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new("security_stamp", user.SecurityStamp ?? string.Empty)
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(minutes);
            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);

            return new RefreshTokenResponseDto(
                tokenString,
                expires,
                newRefreshToken.Token,
                newRefreshToken.ExpiresAt
            );
        }

        private async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken token, string ipAddress)
        {
            var newRefreshToken = new RefreshToken
            {
                UserId = token.UserId,
                Token = GenerateSecureRandomToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress
            };

            // Revoke old token
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplacedByToken = newRefreshToken.Token;

            _context.Set<RefreshToken>().Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return newRefreshToken;
        }

        public async Task RevokeTokenAsync(string refreshToken, string ipAddress)
        {
            var token = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || !token.IsActive)
            {
                _logger.LogWarning("Attempt to revoke invalid or expired refresh token");
                throw new InvalidRefreshTokenException();
            }

            // Revoke token
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked for user: {UserId}", token.UserId);
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                isEmailConfirmed = user.EmailConfirmed
            };
        }

        public async Task<AuthResponse> GoogleLoginAsync(GoogleAuthRequest request)
        {
            // Verify Google ID token
            var googleUserInfo = await VerifyGoogleTokenAsync(request.IdToken);

            if (googleUserInfo == null)
            {
                _logger.LogWarning("Invalid Google ID token");
                throw new InvalidOperationException("Invalid Google ID token");
            }

            // Find user by email or create new one
            var user = await _userManager.FindByEmailAsync(googleUserInfo.Email);

            if (user == null)
            {
                // Create new user from Google account
                user = new User
                {
                    UserName = googleUserInfo.Email,
                    Email = googleUserInfo.Email,
                    FirstName = request.FirstName ?? googleUserInfo.FirstName ?? "User",
                    LastName = request.LastName ?? googleUserInfo.LastName ?? "User",
                    EmailConfirmed = googleUserInfo.EmailVerified
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create user: {errors}");
                }

                // Add to default role
                await _userManager.AddToRoleAsync(user, Roles.User);

                // Add Google login
                var loginInfo = new UserLoginInfo("Google", googleUserInfo.GoogleId, "Google");
                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!addLoginResult.Succeeded)
                {
                    _logger.LogError("Failed to add Google login for user: {Email}", user.Email);
                }

                _logger.LogInformation("New user created via Google login: {Email}", user.Email);
            }
            else
            {
                // Check if Google login is already linked
                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleUserInfo.GoogleId))
                {
                    // Link Google account to existing user
                    var loginInfo = new UserLoginInfo("Google", googleUserInfo.GoogleId, "Google");
                    var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                    if (!addLoginResult.Succeeded)
                    {
                        _logger.LogError("Failed to link Google account for user: {Email}", user.Email);
                    }
                    else
                    {
                        _logger.LogInformation("Google account linked to existing user: {Email}", user.Email);
                    }
                }

                // If email not confirmed yet but Google says it's verified, confirm it
                if (!user.EmailConfirmed && googleUserInfo.EmailVerified)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("Email confirmed via Google for user: {Email}", user.Email);
                }
            }

            _logger.LogInformation("User logged in via Google: {Email}", user.Email);

            return await GenerateAuthResponseAsync(user, "unknown");
        }

        private async Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                // Call Google's tokeninfo endpoint to verify the token
                var response = await httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google token verification failed with status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var tokenInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (tokenInfo == null)
                {
                    _logger.LogWarning("Failed to parse Google token response");
                    return null;
                }

                // Verify the token is for our app (optional but recommended)
                var clientId = _config["Authentication:Google:ClientId"];
                if (!string.IsNullOrEmpty(clientId))
                {
                    if (!tokenInfo.TryGetValue("aud", out var aud) || aud.GetString() != clientId)
                    {
                        _logger.LogWarning("Google token audience mismatch");
                        return null;
                    }
                }

                // Extract user info
                var email = tokenInfo.TryGetValue("email", out var e) ? e.GetString() : null;
                var emailVerified = tokenInfo.TryGetValue("email_verified", out var ev) && ev.GetString() == "true";
                var googleId = tokenInfo.TryGetValue("sub", out var sub) ? sub.GetString() : null;
                var givenName = tokenInfo.TryGetValue("given_name", out var gn) ? gn.GetString() : null;
                var familyName = tokenInfo.TryGetValue("family_name", out var fn) ? fn.GetString() : null;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
                {
                    _logger.LogWarning("Google token missing required fields");
                    return null;
                }

                return new GoogleUserInfo(
                    email,
                    givenName,
                    familyName,
                    googleId,
                    emailVerified
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Google token");
                return null;
            }
        }

    }
}
