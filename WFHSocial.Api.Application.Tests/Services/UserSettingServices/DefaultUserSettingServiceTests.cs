using WFHSocial.Api.Application.Services.UserSettingsServices;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Application.Tests.Services.UserSettingServices
{
    public class DefaultUserSettingServiceTests
    {
        private readonly DefaultUserSettingService _defaultUserSettingService;

        public DefaultUserSettingServiceTests()
        {
            _defaultUserSettingService = new DefaultUserSettingService();
        }

        [Fact]
        public void Test_Success_AddNewDefaultUserSettings()
        {
            var newUser = new ApplicationUser { UserSettings = new List<UserSetting>() };

            _defaultUserSettingService.AddNewDefaultUserSettings(newUser);

            Assert.NotEmpty(newUser.UserSettings);
            Assert.Equal(Enum.GetValues(typeof(UserSettingType)).Length - 1, newUser.UserSettings.Count); // Excluding Unknown
        }

        [Fact]
        public void Test_Failure_AddNewDefaultUserSettings()
        {
            var newUser = new ApplicationUser { UserSettings = new List<UserSetting>() };

            _defaultUserSettingService.AddNewDefaultUserSettings(newUser);

            Assert.NotNull(newUser.UserSettings);
            Assert.NotEmpty(newUser.UserSettings);
        }

        [Fact]
        public void Test_Success_UpdateUserSettings_Succes()
        {
            var user = new ApplicationUser
            {
                UserSettings = new List<UserSetting>
                {
                    new UserSetting { SettingName = "Notifications", IsOn = false }
                }
            };

            _defaultUserSettingService.UpdateUserSettings(user);

            Assert.NotEmpty(user.UserSettings);
            Assert.True(user.UserSettings.First(x => x.SettingName == "Notifications").IsOn);
        }

        [Fact]
        public void Test_FailureUpdate_UserSettings()
        {
            var user = new ApplicationUser
            {
                UserSettings = new List<UserSetting>
                {
                    new UserSetting { SettingName = "MadeUpSetting", IsOn = false }
                }
            };

            _defaultUserSettingService.UpdateUserSettings(user);

            Assert.NotEmpty(user.UserSettings);
            Assert.Contains(user.UserSettings, x => x.SettingName == "MadeUpSetting");
        }

        [Fact]
        public void Test_GetUserSettings_Success()
        {
            var result = _defaultUserSettingService.GetUserSettings();

            Assert.NotEmpty(result);
            Assert.Equal(Enum.GetValues(typeof(UserSettingType)).Length - 1, result.Count);
        }  
    }
}
