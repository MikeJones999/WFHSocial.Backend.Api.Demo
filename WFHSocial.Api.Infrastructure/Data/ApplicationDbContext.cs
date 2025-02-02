using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<UserSetting> UserSettings { get; set; }
    }
}
