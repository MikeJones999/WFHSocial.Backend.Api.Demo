using WFHSocial.Api.Domain.Users.DTOs.AuthModels;

namespace WFHSocial.Api.Application.Interfaces.Services
{
    public interface ILoginAndRegisterUserService
    {
        Task<LoginResponse> LoginUserAsync(UserLogin userLogin);
        Task<RegisterResponse> RegisterNewUserAsync(UserRegister userRegister);
    }
}
