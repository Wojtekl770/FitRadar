using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs.Auth
{
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
    public record VerifyEmailRequest(string Email, string Code);
    public record LoginRequest(string Email, string Password);

    public record AuthResponse(
        string Token,
        DateTime ExpiresUtc,
        UserDto User,
        string RefreshToken,
        DateTime RefreshTokenExpiresUtc
    );
}
