using Microsoft.EntityFrameworkCore;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WFHSocial.Api.Domain.Users.Models;
using WFHSocial.Api.Domain.Users.Models.AuthenticationModels;
using WFHSocial.Api.Domain.Users.Projections.Users;

namespace WFHSocial.Api.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            user.CreatedDateUtc = DateTime.UtcNow;
            user.IsActive = true;
            return await SaveAsync();
        }

        public async Task<ApplicationUser?> GetUserByUserIdAsync(Guid userId)
        {
            return await _dbContext.Users.
                Include(s => s.UserSettings)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
        }

        public async Task<ApplicationUser?> GetUserByUserIdReadonlyAsync(Guid userId)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Include(s => s.UserSettings)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
        }

        public void UpdateSettings(List<UserSetting> settings)
        {
            _dbContext.UserSettings.UpdateRange(settings);
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<AuthorProjection>> GetListOfUserNamesAndIdsByProvidedIdsReadOnlyAsync(List<Guid> authorIds)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(x => authorIds.Contains(Guid.Parse(x.Id)))
                .Select(x => new AuthorProjection
                {
                    AuthorId = Guid.Parse(x.Id),
                    AuthorName = x.UserName!
                }).ToListAsync();
        }

        public Task<List<AuthorProjection>> GetAllAssociatedUsersByUserIdAsync(Guid userId)
        {
            return new Task<List<AuthorProjection>>(() => new List<AuthorProjection>());
        }
    }
}
