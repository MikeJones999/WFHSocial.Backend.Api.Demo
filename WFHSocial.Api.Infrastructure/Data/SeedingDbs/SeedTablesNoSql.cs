using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WFHSocial.Api.Infrastructure.NoSqlConnection.Constants;

namespace WFHSocial.Api.Infrastructure.Data.SeedingDbs
{
    public class SeedTablesNoSql : IHostedService
    {
        private IServiceProvider _serviceProvider;

        public SeedTablesNoSql(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ILogger<SeedTablesNoSql> logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedTablesNoSql>>();
                Database? cosmosDatabase = scope.ServiceProvider.GetRequiredService<Database>();
                try
                {
                    NoSqlContainerSetupProperties.Containers.ForEach(async noSqlContainer =>
                    {
                        Container cosmosContainer = await cosmosDatabase.CreateContainerIfNotExistsAsync(noSqlContainer.GetContainerDefault(),
                            noSqlContainer.GetThroughPutProperties(),
                            null,
                            cancellationToken);
                        logger.LogInformation("WFH - Successfully validated {Container} container - and has been created if required", noSqlContainer.GetContainerDefault().Id);
                    });
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "WFH - Error creating container during the seeding process");
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
