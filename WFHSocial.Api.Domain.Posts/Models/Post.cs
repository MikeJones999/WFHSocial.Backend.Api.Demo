using System.Text.Json.Serialization;

namespace WHFSocial.Api.Domain.Posts.Models
{
    public class Post
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public DateTime PostDateUtc { get; set; } = DateTime.UtcNow;
        public string PostContent { get; set; } = string.Empty;
        public PostType PostType { get; set; }
        public List<string> Comments { get; set; } = new List<string>();
        //public List<string> Tags { get; set; } = new List<string>();

    }
}
