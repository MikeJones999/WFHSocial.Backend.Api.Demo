﻿
using Microsoft.AspNetCore.Identity;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Domain.Authentication.Models
{
    public class ApplicationUser: IdentityUser
    {
        public DateTime CreatedDateUtc { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? DateModifiedUtc { get; set; }
        public bool IsRecruiter { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public bool HasProfilePicture { get; set; }
        public List<UserSetting> UserSettings { get; set; } = new List<UserSetting>();

        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
