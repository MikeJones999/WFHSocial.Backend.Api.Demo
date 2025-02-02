using Microsoft.Azure.Cosmos;

namespace WFHSocial.Api.Infrastructure.NoSqlConnection
{
    public abstract class NoSqlContainer
    {
        protected IndexingPolicy GetindexingPolicy(bool isAutomatic = false, IndexingMode indexingMode = IndexingMode.Lazy)
        {
            return new IndexingPolicy()
            {
                Automatic = isAutomatic,
                IndexingMode = indexingMode,
            };
        }
    }
}