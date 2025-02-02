namespace WFHSocial.Api.Domain.Users.DTOs.UserProfileModels
{
    public class UserProfileResponse
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<UserSettingView> SettingViews { get; set; } = new List<UserSettingView>();
        public bool HasProfileImage { get; set; }
    }
}
