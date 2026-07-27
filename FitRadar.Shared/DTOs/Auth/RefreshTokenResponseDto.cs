using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs.Auth
{
    /// <summary>
    /// Request for refreshing JWT access token.
    /// </summary>
    public record RefreshTokenRequestDto(string RefreshToken);

    /// <summary>
    /// Response containing new access token and refresh token.
    /// </summary>
    public record RefreshTokenResponseDto(
        string AccessToken,
        DateTime AccessTokenExpiresUtc,
        string RefreshToken,
        DateTime RefreshTokenExpiresUtc
    );
}
