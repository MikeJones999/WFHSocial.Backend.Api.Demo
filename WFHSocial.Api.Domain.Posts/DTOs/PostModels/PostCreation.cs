using System.ComponentModel.DataAnnotations;

namespace WFHSocial.Api.Domain.Posts.DTOs.PostModels
{
    public class PostCreationRequest
    {
        //    [Required]
        //    public string AuthorName { get; set; } = string.Empty;
        //    [Required]
        //    public string AuthorId { get; set; } = string.Empty;
        [Required]
        public DateTime PostDateUtc { get; set; } = DateTime.UtcNow;
        [Required]
        public string PostContent { get; set; } = string.Empty;
        [Required]
        public PostTypeDto PostType { get; set; }
    }

    public class PostCreationResponse
    {
        public Guid Id { get; set; }
    }
}
