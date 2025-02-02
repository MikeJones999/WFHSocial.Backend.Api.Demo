namespace WFHSocial.Api.Domain.Users.DTOs.UserProfileModels
{
    public class UserProfileUpdateRequest
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public List<UserSettingView> SettingViews { get; set; } = new List<UserSettingView>();
    }
}
