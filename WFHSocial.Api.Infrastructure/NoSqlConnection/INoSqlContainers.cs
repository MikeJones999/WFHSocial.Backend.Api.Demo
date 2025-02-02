using Microsoft.Azure.Cosmos;

namespace WFHSocial.Api.Infrastructure.NoSqlConnection
{
    public interface INoSqlContainers
    {
        ContainerProperties GetContainerDefault();
        ThroughputProperties GetThroughPutProperties();
    }
}
