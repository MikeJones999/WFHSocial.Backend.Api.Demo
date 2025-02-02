using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WFHSocial.Api.Infrastructure.NoSqlConnection;

namespace WFHSocial.Api.Infrastructure.Data.SeedingDbs
{
    public class ApplicationDbContextSeedMigrations : IHostedService
    {
        private IServiceProvider _serviceProvider;

        public ApplicationDbContextSeedMigrations(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ILogger<ApplicationDbContextSeedMigrations> logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContextSeedMigrations>>();
                ApplicationDbContext? dcContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    await dcContext.Database.EnsureCreatedAsync();
                    await dcContext.Database.MigrateAsync();
                    logger.LogInformation("WFH - Successfully validated ApplicationDbContext sql Server - and has been created if required");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "WFH - Error creating Sql Database during the seeding process");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested && cancellationToken.CanBeCanceled)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            return Task.CompletedTask;
        }
    }

}
