using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Application.Interfaces.Services
{
    public interface IDefaultUserSettingService
    {
        void AddNewDefaultUserSettings(ApplicationUser newUser);
        List<UserSetting> GetUserSettings();
        void UpdateUserSettings(ApplicationUser user);
    }
}