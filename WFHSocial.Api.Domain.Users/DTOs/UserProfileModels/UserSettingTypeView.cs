using System.ComponentModel;

namespace WFHSocial.Api.Domain.Users.DTOs.UserProfileModels
{
    public enum UserSettingTypeView
    {
        Unknown = 0,
        [PropertyTab("Notifications")]
        Notifications = 1, // "Notifications";
        [PropertyTab("AllowForEmailsFromWFH")]
        AllowForEmailsFromWFH = 2, // "All emails from WFH system";
        [PropertyTab("BlockOutInappropriateLanguage")]
        BlockOutInappropriateLanguage = 3, //"Block/edit inappropriate language";
        [PropertyTab("AllowUseOfProfilePicture")]
        AllowUseOfProfilePicture = 4, //"Allow use of profile picture";
    }
}
