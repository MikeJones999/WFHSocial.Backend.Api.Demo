using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WFHSocial.Api.Infrastructure.Data.SeedingDbs
{
    public static class ValidateAnd
    {
        public static IServiceCollection ValidateAndSeedDatabases(this IServiceCollection services, IConfiguration configuration, string environmentName)
        {
            return CheckAndSeedDatabases(services, configuration);

        }
        private static IServiceCollection CheckAndSeedDatabases(IServiceCollection services, IConfiguration configuration)
        {
            string? seedDatabase = configuration.GetSection("Database:Seed").Value!;
            string? seedNoSql = configuration.GetSection("CosmosDB:Seed").Value!;
            if (!string.IsNullOrEmpty(seedDatabase) && bool.Parse(seedDatabase))
            {
                services.AddHostedService<ApplicationDbContextSeedMigrations>();
            }
   
            if (!string.IsNullOrEmpty(seedNoSql) && bool.Parse(seedNoSql))
            {
                services.AddHostedService<SeedTablesNoSql>();
            }

            return services;
        }
    }
    
}
