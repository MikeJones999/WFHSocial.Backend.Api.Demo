using Microsoft.AspNetCore.Http;
using WFHSocial.Api.Domain.Users.DTOs.UserProfileModels;
using WFHSocial.Shared.FileUploads;

namespace WFHSocial.Api.Application.Interfaces.Services
{
    public interface IUserProfileAndSettingsService
    {
        Task<UserProfileResponse?> GetUserProfileAndSettingsAsync(Guid userId);
        Task<UploadResult> UpdateProfileImageAsync(IFormFile file, Guid userId);
        Task<UserProfileUpdateResponse> UpdateUserProfileAndSettingsAsync(UserProfileUpdateRequest userProfile);
    }
}