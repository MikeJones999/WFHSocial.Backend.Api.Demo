using Microsoft.Azure.Cosmos;
using WFHSocial.Api.Infrastructure.NoSqlConnection.Constants;

namespace WFHSocial.Api.Infrastructure.NoSqlConnection
{
    public class NoSqlContainerPeople : NoSqlContainer, INoSqlContainers
    {
        private int _throughput = 400;
        public ThroughputProperties GetThroughPutProperties()
        {
            return ThroughputProperties.CreateAutoscaleThroughput(_throughput);
        }

        public ContainerProperties GetContainerDefault()
        {
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = NoSqlContainerSetupProperties.People,
                PartitionKeyPath = NoSqlContainerSetupProperties.DefaultPartitionKey,
                IndexingPolicy = GetindexingPolicy()
            };

            return containerProperties;
        }
    }
}
