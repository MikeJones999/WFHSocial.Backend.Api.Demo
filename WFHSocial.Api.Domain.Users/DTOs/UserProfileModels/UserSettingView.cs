namespace WFHSocial.Api.Domain.Users.DTOs.UserProfileModels
{
    public class UserSettingView
    {
        public Guid Id { get; set; }
        public bool IsOn { get; set; }
        public UserSettingTypeView SettingType { get; set; }
        public string SettingName { get; init; } = string.Empty;  //change to get from enum property
        public DateTime DateTimeModifiedUtc { get; set; }
    }
}
