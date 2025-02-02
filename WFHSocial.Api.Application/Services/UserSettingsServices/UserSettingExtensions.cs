using System.ComponentModel;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Application.Services.UserSettingsServices
{
    public static class UserSettingExtensions
    {
        public static string? GetName(this UserSettingType value)
        {
            return Enum.GetName(typeof(UserSettingType), value);
        }

        public static T? GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static string GetAttributeDescription(this UserSettingType enumValue)
        {
            DescriptionAttribute? attribute = enumValue.GetAttributeOfType<DescriptionAttribute>();
            return attribute == null ? string.Empty : attribute.Description;
        }
    }
}
