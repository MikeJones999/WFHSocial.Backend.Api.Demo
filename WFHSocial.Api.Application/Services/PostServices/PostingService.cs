using AutoMapper;
using Microsoft.Extensions.Logging;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Application.MappingProfiles;
using WFHSocial.Api.Domain.Posts.DTOs.PostModels;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WFHSocial.Api.Domain.Users.Projections.Users;
using WFHSocial.Utility.StaticHelpers;
using WHFSocial.Api.Domain.Posts.Interfaces.Repositories;
using WHFSocial.Api.Domain.Posts.Models;

namespace WFHSocial.Api.Application.Services.PostServices
{
    public class PostingService : IPostingService
    {
        private readonly ILogger<PostingService> _logger;
        private readonly IPostRepositoryNoSql _postRepositoryNoSql;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public PostingService(ILogger<PostingService> logger, IPostRepositoryNoSql postRepositoryNoSql, IMapper mapper, IUserRepository userRepository)
        {
            _logger = logger;
            _postRepositoryNoSql = postRepositoryNoSql;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<PostDto?> GetUsersPersonalPostByIdAsync(Guid userId, Guid postId)
        {
            Post? post = await _postRepositoryNoSql.GetUsersPersonalPostByIdAsync(userId, postId);
            if (post != null)
            {
                PostDto internalObject = _mapper.Map<PostDto>(post);
                return internalObject;
            }
            return null;
        }

        public async Task<Guid?> CreateNewUserPostByUserIdAsync(Guid UserId, PostCreationRequest creationRequest)
        {
            if (InputValidation.ContainsHtmlOrScript(creationRequest.PostContent))
            {
                _logger.LogWarning("Post content contains html or script tags. Request method: {Method}", nameof(this.CreateNewUserPostByUserIdAsync));
                return null;
            }

            Post post = new Post()
            {
                AuthorId = UserId.ToString(),
                PostDateUtc = DateTime.UtcNow,
                PostContent = creationRequest.PostContent,
                PostType = PostMappingProfiles.MapPostTypeDtoToPostType(creationRequest.PostType)
            };

            Guid? postId = await _postRepositoryNoSql.CreateNewPostAsync(post);
            return postId;
        }

        public async Task<ListPostDto> GetPaginatedDescOrderedUserPostsByUserIdAsync(Guid userId, string userName, GetPostListFilter filters)
        {
            (List<Post>? posts, int total) = await _postRepositoryNoSql.GetUsersPersonalPostsByUserIdAsync(userId, filters);
            List<PostDto> postDtos = new List<PostDto>();
            if (posts != null && posts.Any())
            {
                postDtos = posts.Select(x => _mapper.Map<PostDto>(x)).ToList();
                Parallel.ForEach(postDtos, x => {                    
                    x.AuthorName = userName;
                });
            }
            ListPostDto returnPosts = new ListPostDto()
            {
                TotalPostsStored = total,
                Posts = postDtos
            };

            return returnPosts;
        }

        public async Task<ListPostDto> GetPaginatedDescOrderedRecentPostsByAllAssociatedUsersIdAsync(Guid userId, GetPostListFilter filters)
        {
            //TODO Get a list of Associates (Not yet implemented) - Authors struct
            List<AuthorProjection> authorIds = await _userRepository.GetAllAssociatedUsersByUserIdAsync(userId);
            ListPostDto returnPosts = new ListPostDto(){ TotalPostsStored = 0 };

            if (authorIds == null || !authorIds.Any())
            {
                return returnPosts;
            }

            (List<Post>? posts, int total) = await _postRepositoryNoSql.GetPostsByUsersByListOfIdsAsync(authorIds.Select(x => x.AuthorId).ToList(), filters);
            List<PostDto> postDtos = new List<PostDto>();
            if (posts != null && posts.Any())
            {
                postDtos = posts.Select(x => _mapper.Map<PostDto>(x)).ToList();
                Parallel.ForEach(postDtos, x => {
                    x.AuthorName = authorIds.FirstOrDefault(p => p.AuthorId == x.Id).AuthorName;
                });
            }

            returnPosts.TotalPostsStored = total;
            returnPosts.Posts = postDtos;  
            return returnPosts;
        }
    }
}

        