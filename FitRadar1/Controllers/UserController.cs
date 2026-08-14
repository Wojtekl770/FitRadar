using FitRadar.Services.Exceptions;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace FitRadar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Register new user and send verification email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                await _userService.RegisterAsync(request);
                return Ok(new { message = "Registration successful. Check your email for verification code." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Registration validation error");
                return BadRequest(new ErrorResponse("VALIDATION_ERROR", ex.Message));
            }
            catch (EmailAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, "Email already exists");
                return BadRequest(new ErrorResponse("EMAIL_EXISTS", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed");
                return BadRequest(new ErrorResponse("REGISTRATION_FAILED", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return StatusCode(500, new ErrorResponse("INTERNAL_ERROR", "An error occurred during registration. Please try again later."));
            }
        }

        /// <summary>
        /// Email verification endpoint. Verifies the user's email using the provided code and returns JWT and refresh token in cookies.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("verify-email")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            try
            {
                var userResponse = await _userService.VerifyEmailAsync(request);
                SetTokenCookies(userResponse.Token, userResponse.ExpiresUtc, userResponse.RefreshToken, userResponse.RefreshTokenExpiresUtc);
                return Ok(userResponse);
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found during email verification");
                return BadRequest(new ErrorResponse("USER_NOT_FOUND", ex.Message));
            }
            catch (InvalidVerificationCodeException ex)
            {
                _logger.LogWarning(ex, "Invalid verification code");
                return BadRequest(new ErrorResponse("INVALID_CODE", ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Verification validation error");
                return BadRequest(new ErrorResponse("VALIDATION_ERROR", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Verification failed");
                return BadRequest(new ErrorResponse("VERIFICATION_FAILED", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during email verification");
                return StatusCode(500, new ErrorResponse("INTERNAL_ERROR", "An error occurred during email verification. Please try again later."));
            }
        }

        /// <summary>
        /// User login. Returns JWT token and user data.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var ipAddress = GetIpAddress();
                var authResponse = await _userService.LoginAsync(req);

                SetTokenCookies(authResponse.Token, authResponse.ExpiresUtc, authResponse.RefreshToken, authResponse.RefreshTokenExpiresUtc);

                return Ok(authResponse);
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found during login");
                return Unauthorized(new ErrorResponse("USER_NOT_FOUND", ex.Message));
            }
            catch (InvalidPasswordException ex)
            {
                _logger.LogWarning(ex, "Invalid password during login for email: {Email}", req.Email);
                return Unauthorized(new ErrorResponse("INVALID_PASSWORD", ex.Message));
            }
            catch (EmailNotVerifiedException ex)
            {
                _logger.LogWarning(ex, "Email not verified during login for email: {Email}", req.Email);
                return BadRequest(new ErrorResponse("EMAIL_NOT_VERIFIED", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return StatusCode(500, new ErrorResponse("INTERNAL_ERROR", "An error occurred. Please try again later."));
            }
        }

        /// <summary>
        /// Refreshes JWT access token using refresh token.
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshTokenResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["RefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new ErrorResponse("MISSING_TOKEN", "Refresh token is required."));
                }

                var ipAddress = GetIpAddress();
                var response = await _userService.RefreshTokenAsync(refreshToken, ipAddress);

                SetTokenCookies(response.AccessToken, response.AccessTokenExpiresUtc, response.RefreshToken, response.RefreshTokenExpiresUtc);

                return Ok(response);
            }
            catch (InvalidRefreshTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid refresh token");
                return Unauthorized(new ErrorResponse("INVALID_REFRESH_TOKEN", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token refresh");
                return StatusCode(500, new ErrorResponse("INTERNAL_ERROR", "An error occurred. Please try again later."));
            }
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RevokeTokenAsync()
        {
            try
            {
                var refreshToken = Request.Cookies["RefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new ErrorResponse("MISSING_TOKEN", "Refresh token is required."));
                }

                var ipAddress = GetIpAddress();
                await _userService.RevokeTokenAsync(refreshToken, ipAddress);

                Response.Cookies.Delete("JWT");
                Response.Cookies.Delete("RefreshToken");

                return Ok(new { message = "Token revoked successfully." });
            }
            catch (InvalidRefreshTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid refresh token during revocation");
                return BadRequest(new ErrorResponse("INVALID_REFRESH_TOKEN", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token revocation");
                return StatusCode(500, new ErrorResponse("INTERNAL_ERROR", "An error occurred. Please try again later."));
            }
        }

        /// <summary>
        /// User logout (stateless JWT — client must delete the token).
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWT");
            Response.Cookies.Delete("RefreshToken");
            _logger.LogInformation("User logged out (JWT and refresh token cookies deleted)");
            return Ok(new { message = "Logged out successfully." });
        }

        /// <summary>
        /// Login or register using Google account
        /// </summary>
        /// <param name="request">Google ID token and optional user info</param>
        /// <returns>Authentication response with JWT tokens</returns>
        [HttpPost("google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.IdToken))
                {
                    return BadRequest(new ErrorResponse("MISSING_TOKEN", "Google ID token is required"));
                }

                var response = await _userService.GoogleLoginAsync(request);

                // Set tokens in HTTP-only cookies
                SetTokenCookies(response.Token, response.ExpiresUtc, response.RefreshToken, response.RefreshTokenExpiresUtc);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Google login validation error");
                return BadRequest(new ErrorResponse("GOOGLE_LOGIN_FAILED", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Google login");
                return StatusCode(500, new ErrorResponse("INTERNAL_ERROR", "An error occurred during Google login"));
            }
        }



        // Helper method to set JWT and refresh token cookies
        private void SetTokenCookies(string accessToken, DateTime accessTokenExpires, string refreshToken, DateTime refreshTokenExpires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            // Set access token cookie
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = accessTokenExpires
            };
            Response.Cookies.Append("JWT", accessToken, accessCookieOptions);

            // Set refresh token cookie
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenExpires
            };
            Response.Cookies.Append("RefreshToken", refreshToken, refreshCookieOptions);
        }

        private string GetIpAddress()
        {
            // Get IP address from X-Forwarded-For header or connection
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        /// <summary>
        /// Standardized error response for API
        /// </summary>
        public record ErrorResponse(string Code, string Message);
    }
}
