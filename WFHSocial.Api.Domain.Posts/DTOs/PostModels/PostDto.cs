using System.Text.Json.Serialization;

namespace WFHSocial.Api.Domain.Posts.DTOs.PostModels
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public DateTime PostDateUtc { get; set; } = DateTime.UtcNow;
        public string PostContent { get; set; } = string.Empty;
        public PostTypeDto PostType { get; set; }
        public List<string> Comments { get; set; } = new List<string>();
        //public List<string> Tags { get; set; } = new List<string>();
    }
}
