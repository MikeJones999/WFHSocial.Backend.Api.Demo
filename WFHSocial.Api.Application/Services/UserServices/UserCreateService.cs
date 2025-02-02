using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.DTOs.AuthModels;

namespace WFHSocial.Api.Application.Services.UserServices
{
    public static class UserCreateService
    {
        public static ApplicationUser CreateNewUser(UserRegister userRegister)
        {
            ApplicationUser newUser = new ApplicationUser();
            newUser.DisplayName = string.IsNullOrWhiteSpace(userRegister.DisplayName) ? userRegister.UserName : userRegister.DisplayName;
            newUser.Email = userRegister.Email;
            newUser.IsRecruiter = userRegister.IsRecruiter;
            newUser.UserName = userRegister.UserName;
            return newUser;
        }
    }
}
