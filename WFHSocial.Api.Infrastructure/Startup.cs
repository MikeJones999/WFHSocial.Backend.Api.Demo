using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using WFHSocial.Api.Application.Interfaces.Repository;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WFHSocial.Api.Infrastructure.BlobContainerAccess;
using WFHSocial.Api.Infrastructure.Data;
using WFHSocial.Api.Infrastructure.Data.Repositories;
using WHFSocial.Api.Domain.Posts.Interfaces.Repositories;

namespace WFHSocial.Api.Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddTransient<IUserRepository,UserRepository>();
            services.AddTransient<IBlobServices, BlobServices>();
            services.AddTransient<IUserRepositoryNoSql, UserRepositoryNoSql>();
            services.AddTransient<IPostRepositoryNoSql, PostRepositoryNoSql>();

            AddCosmosDbConnection(services, configuration);
            return services;
        }
  
        public static void AddCosmosDbConnection(IServiceCollection services, IConfiguration configuration)
        {
            var endpoint = configuration.GetValue<string>("CosmosDB:Endpoint");
            var PrimaryKey = configuration.GetValue<string>("CosmosDB:PrimaryKey");
            var database = configuration.GetValue<string>("CosmosDB:Database");
            var containerName = configuration.GetValue<string>("CosmosDB:Container");

            SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
            // Customize this value based on desired DNS refresh timer
            socketsHttpHandler.PooledConnectionLifetime = TimeSpan.FromMinutes(5);
            // Registering the Singleton SocketsHttpHandler lets you reuse it across any HttpClient in your application
            services.AddSingleton<SocketsHttpHandler>(socketsHttpHandler);
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            // Use a Singleton instance of the CosmosClient
            services.AddSingleton<Database>(serviceProvider =>
            {
                SocketsHttpHandler socketsHttpHandler = serviceProvider.GetRequiredService<SocketsHttpHandler>();
                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Direct,
                    HttpClientFactory = () => new HttpClient(socketsHttpHandler, disposeHandler: false),
                    Serializer = new WFHSocial.Api.Infrastructure.NoSqlConnection.CosmosSystemTextJsonSerializer(options),
                };

                CosmosClient client = new CosmosClient(endpoint, PrimaryKey, cosmosClientOptions);
                Database cosmosDb = client.GetDatabase(database);
                return cosmosDb;
            });

        }

    }
}
