using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Application.Services.UserSettingsServices
{
    public class DefaultUserSettingService : IDefaultUserSettingService
    {
        public void AddNewDefaultUserSettings(ApplicationUser newUser)
        {
            newUser.UserSettings.AddRange(GetUserSettings());
        }

        public void UpdateUserSettings(ApplicationUser user)
        {
            List<UserSetting> userSettings = GetUserSettings();
            foreach (UserSetting setting in userSettings)
            {
                UserSetting? existingSetting = user.UserSettings.Find(x => x.SettingName == setting.SettingName);
                if(existingSetting is not null)
                {
                    existingSetting.IsOn = setting.IsOn;
                }
                else
                {
                    user.UserSettings.Add(setting);
                }
            }
        }

        public List<UserSetting> GetUserSettings()
        {
            List<UserSetting> userSettings = new List<UserSetting>();
            var settingTypes = Enum.GetValues(typeof(UserSettingType));
            foreach(UserSettingType setting in settingTypes)
            {
                if(setting != UserSettingType.Unknown)
                {
                    UserSettingType settingType = (UserSettingType)setting;
                    userSettings.Add(new UserSetting
                    {
                        IsOn = true,
                        SettingName = settingType.GetAttributeDescription(),
                        SettingType = settingType,
                        DateTimeModifiedUtc = DateTime.UtcNow,
                    });
                }            
            }         
            return userSettings;
        }
    }
}
