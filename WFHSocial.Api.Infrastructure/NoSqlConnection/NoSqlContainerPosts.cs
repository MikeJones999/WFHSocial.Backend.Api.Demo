using Microsoft.Azure.Cosmos;
using WFHSocial.Api.Infrastructure.NoSqlConnection.Constants;

namespace WFHSocial.Api.Infrastructure.NoSqlConnection
{
    public class NoSqlContainerPosts : NoSqlContainer, INoSqlContainers
    {
        private int _throughput = 10000;
        public ThroughputProperties GetThroughPutProperties()
        {
            return ThroughputProperties.CreateAutoscaleThroughput(_throughput);
        }

        public ContainerProperties GetContainerDefault()
        {
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = NoSqlContainerSetupProperties.Posts,
                PartitionKeyPath = NoSqlContainerSetupProperties.DefaultPartitionKey,
                IndexingPolicy = GetindexingPolicy()
            };

            return containerProperties;
        }
    }
}
