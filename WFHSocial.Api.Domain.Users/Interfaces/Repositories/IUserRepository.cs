using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Models;
using WFHSocial.Api.Domain.Users.Models.AuthenticationModels;
using WFHSocial.Api.Domain.Users.Projections.Users;

namespace WFHSocial.Api.Domain.Users.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync(User user);
        Task<ApplicationUser?> GetUserByUserIdAsync(Guid userId);
        void UpdateSettings(List<UserSetting> settings);
        Task<bool> SaveAsync();
        Task<ApplicationUser?> GetUserByUserIdReadonlyAsync(Guid userId);
        Task<List<AuthorProjection>> GetListOfUserNamesAndIdsByProvidedIdsReadOnlyAsync(List<Guid> authorIds);
        Task<List<AuthorProjection>> GetAllAssociatedUsersByUserIdAsync(Guid userId);
    }
}