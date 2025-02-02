using WFHSocial.Api.Domain.Posts.DTOs.PostModels;
using WHFSocial.Api.Domain.Posts.Models;

namespace WHFSocial.Api.Domain.Posts.Interfaces.Repositories
{
    public interface IPostRepositoryNoSql
    {
        Task<Guid?> CreateNewPostAsync(Post post);
        Task<List<Post>> GetUsersPersonalPostsAsync(Guid userId);
        Task<Post?> GetUsersPersonalPostByIdAsync(Guid userId, Guid postId);
        Task<(List<Post>? posts, int total)> GetUsersPersonalPostsByUserIdAsync(Guid userId, GetPostListFilter filter);
        Task<int> GetPostCountAsync(Guid userId);
        Task<(List<Post>? posts, int total)> GetPostsByUsersByListOfIdsAsync(List<Guid> authorIds, GetPostListFilter filters);
    }
}