namespace WFHSocial.Api.Domain.Posts.DTOs.PostModels
{
    public class GetPostListFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IsDescendingByDate { get; set; } = true;
        public int Getskip()
        {
            return (Page - 1) * PageSize;
        }
    }
}
