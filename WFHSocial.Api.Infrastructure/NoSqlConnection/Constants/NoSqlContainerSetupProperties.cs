namespace WFHSocial.Api.Infrastructure.NoSqlConnection.Constants
{
    public static class NoSqlContainerSetupProperties
    {
        public const string DefaultPartitionKey = "/id";
        public const string Posts = "Posts";
        public const string People = "People";

        public static List<INoSqlContainers> Containers = new()
        {
            new NoSqlContainerPosts(),
            new NoSqlContainerPeople()
        };
    }


}
