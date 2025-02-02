using System.Text.Json.Serialization;

namespace WHFSocial.NoSqlModels
{
    public class PersonModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
