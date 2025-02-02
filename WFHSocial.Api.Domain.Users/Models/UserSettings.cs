using System.ComponentModel.DataAnnotations;
using WFHSocial.Api.Domain.Authentication.Models;
namespace WFHSocial.Api.Domain.Users.Models
{
    public class UserSetting
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsOn { get; set; }
        public UserSettingType SettingType { get; set; }
        public string SettingName { get; set; } = string.Empty;
        public DateTime DateTimeModifiedUtc { get; set; }

        //Navigation Propertioes
        public ApplicationUser? User { get; set; }
    }
}
