using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs.Auth
{
    /// <summary>
    /// Request DTO for Google authentication
    /// </summary>
    public record GoogleAuthRequest(
        string IdToken,
        string? FirstName = null,
        string? LastName = null
    );

    /// <summary>
    /// Response containing Google user info extracted from token
    /// </summary>
    public record GoogleUserInfo(
        string Email,
        string? FirstName,
        string? LastName,
        string GoogleId,
        bool EmailVerified
    );
}
