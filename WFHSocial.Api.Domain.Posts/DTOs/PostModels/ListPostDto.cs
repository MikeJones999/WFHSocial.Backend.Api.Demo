namespace WFHSocial.Api.Domain.Posts.DTOs.PostModels
{
    public class ListPostDto
    {
        public int TotalPostsStored { get; set; }
        public List<PostDto> Posts { get; set; } = new List<PostDto>();
    }
}
