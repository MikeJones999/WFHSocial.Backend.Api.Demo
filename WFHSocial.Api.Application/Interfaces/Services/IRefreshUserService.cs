using WFHSocial.Api.Domain.Users.DTOs.AuthModels;

namespace WFHSocial.Api.Application.Interfaces.Services
{
    public interface IRefreshUserService
    {
        Task<RefreshResponse> RefreshTokenAsync(Guid userId, RefreshTokenRequest refreshTokenRequest);
    }
}