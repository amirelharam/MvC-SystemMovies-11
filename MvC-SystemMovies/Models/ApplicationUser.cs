using Microsoft.AspNetCore.Identity;

namespace MvC_SystemMovies.Models
{
    /// <summary>
    /// Extends the default Identity user with extra profile / OTP / blocking fields
    /// needed by the system (Roles, Block/Unblock, OTP verification).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? ProfileImage { get; set; }

        /// <summary>
        /// Soft "block" flag shown in the Admin UI. The actual enforcement at
        /// login time is done through IdentityUser.LockoutEnabled / LockoutEnd
        /// (see UsersController.Block), this flag just makes the reason explicit
        /// and easy to query/display.
        /// </summary>
        public bool IsBlocked { get; set; } = false;

        /// <summary>
        /// The last One-Time-Password sent to the user (hashed would be better in
        /// production, kept plain here for simplicity of the exercise).
        /// </summary>
        public string? OtpCode { get; set; }

        public DateTime? OtpCodeExpiresOn { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
