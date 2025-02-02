using System.ComponentModel;

namespace WFHSocial.Api.Domain.Users.Models
{
    public enum UserSettingType
    {
        Unknown = 0,
        [Description("Notifications")]
        Notifications = 1, // "Notifications";
        [Description("Allow For Emails From WFH")]
        AllowForEmailsFromWFH = 2, // "All emails from WFH system";
        [Description("Block Out Inappropriate Language")]
        BlockOutInappropriateLanguage = 3, //"Block/edit inappropriate language";
        [Description("Allow Use Of Profile Picture")]
        AllowUseOfProfilePicture = 4, //"Allow use of profile picture";
    }
}
