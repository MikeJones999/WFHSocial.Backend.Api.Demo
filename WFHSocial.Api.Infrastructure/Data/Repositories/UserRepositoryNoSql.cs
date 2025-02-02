using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using WFHSocial.Api.Application.Interfaces.Repository;
using WHFSocial.NoSqlModels;

namespace WFHSocial.Api.Infrastructure.Data.Repositories
{
    public class UserRepositoryNoSql : IUserRepositoryNoSql
    {
        private readonly ILogger<UserRepositoryNoSql> _logger;
        private readonly Database _cosmosDatabase;

        public UserRepositoryNoSql(ILogger<UserRepositoryNoSql> logger, Database cosmosDatabase)
        {
            _logger = logger;
            _cosmosDatabase = cosmosDatabase;
        }

        public async Task<PersonModel?> GetPerson()
        {
            Container cosmosContainer = _cosmosDatabase.GetContainer("People");
            FeedIterator<PersonModel> feedIterator = cosmosContainer.GetItemQueryIterator<PersonModel>();  //all
            PersonModel? person = null;
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<PersonModel> currentResultSet = await feedIterator.ReadNextAsync();
                Console.WriteLine($"RUs for this call: {currentResultSet.RequestCharge}");

                person = currentResultSet.FirstOrDefault();
            }

            return person;
        }

   
    }
}
