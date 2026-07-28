using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.Models
{
    /// <summary>
    /// Class defining constants for role names
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Represents the name of the administrator role which manages website
        /// </summary>
        public const string Admin = nameof(Admin);

        /// <summary>
        /// Represents a regular user
        /// </summary>
        public const string User = nameof(User);
    }
}
