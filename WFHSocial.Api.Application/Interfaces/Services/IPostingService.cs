using WFHSocial.Api.Domain.Posts.DTOs.PostModels;

namespace WFHSocial.Api.Application.Interfaces.Services
{
    public interface IPostingService
    {
        Task<Guid?> CreateNewUserPostByUserIdAsync(Guid UserId, PostCreationRequest creationRequest);
        Task<PostDto?> GetUsersPersonalPostByIdAsync(Guid userId, Guid postId);
        Task<ListPostDto> GetPaginatedDescOrderedUserPostsByUserIdAsync(Guid userId, string userName, GetPostListFilter filters);
    }
}