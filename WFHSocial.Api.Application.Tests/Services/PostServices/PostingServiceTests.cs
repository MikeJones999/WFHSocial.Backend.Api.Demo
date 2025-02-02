using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Application.MappingProfiles;
using WFHSocial.Api.Application.Services.PostServices;
using WFHSocial.Api.Domain.Posts.DTOs.PostModels;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WHFSocial.Api.Domain.Posts.Interfaces.Repositories;
using WHFSocial.Api.Domain.Posts.Models;

namespace WFHSocial.Api.Application.Tests.Services.PostServices
{
    public class PostingServiceTests
    {
        private readonly Mock<ILogger<PostingService>> _loggerMock;
        private readonly Mock<IPostRepositoryNoSql> _postRepositoryNoSqlMock;
        private readonly IMapper _actualMapper;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IPostingService _postingService;

        public PostingServiceTests()
        {
            _loggerMock = new Mock<ILogger<PostingService>>();
            _postRepositoryNoSqlMock = new Mock<IPostRepositoryNoSql>();
            _userRepositoryMock = new Mock<IUserRepository>();

            MapperConfiguration mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<PostMappingProfiles>());
            _actualMapper = mapperConfig.CreateMapper();
            _postingService = new PostingService(_loggerMock.Object, _postRepositoryNoSqlMock.Object, _actualMapper, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Test_Success_GetUsersPersonalPostByIdAsync()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId.ToString(), AuthorId = userId.ToString() };
            var postDto = new PostDto { Id = postId, AuthorId = userId.ToString() };

            _postRepositoryNoSqlMock.Setup(x => x.GetUsersPersonalPostByIdAsync(userId, postId)).ReturnsAsync(post);

            var result = await _postingService.GetUsersPersonalPostByIdAsync(userId, postId);

            _postRepositoryNoSqlMock.Verify(x => x.GetUsersPersonalPostByIdAsync(userId, postId), Times.Once());

            Assert.NotNull(result);
            Assert.Equal(postId, result.Id);
        }

        [Fact]
        public async Task Test_Failure_GetUsersPersonalPostByIdAsync_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            _postRepositoryNoSqlMock.Setup(x => x.GetUsersPersonalPostByIdAsync(userId, postId)).ReturnsAsync((Post?)null);

            var result = await _postingService.GetUsersPersonalPostByIdAsync(userId, postId);

            _postRepositoryNoSqlMock.Verify(x => x.GetUsersPersonalPostByIdAsync(userId, postId), Times.Once());

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_Success_CreateNewUserPostByUserIdAsync()
        {
            var userId = Guid.NewGuid();
            var creationRequest = new PostCreationRequest { PostContent = "Test Content", PostType = PostTypeDto.Personal };
            var postId = Guid.NewGuid();

            _postRepositoryNoSqlMock.Setup(repo => repo.CreateNewPostAsync(It.IsAny<Post>())).ReturnsAsync(postId);

            var result = await _postingService.CreateNewUserPostByUserIdAsync(userId, creationRequest);

            Assert.NotNull(result);
            Assert.Equal(postId, result);
        }

        [Fact]
        public async Task Test_Failure_CreateNewUserPostByUserIdAsync_InvalidHtmlContent()
        {
            var userId = Guid.NewGuid();
            var creationRequest = new PostCreationRequest { PostContent = "<script>alert('test');</script>", PostType = PostTypeDto.Personal };

            var result = await _postingService.CreateNewUserPostByUserIdAsync(userId, creationRequest);

            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals("Post content contains html or script tags. Request method: CreateNewUserPostByUserIdAsync", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
               Times.Once);

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_Success_GetPaginatedDescOrderedUserPostsByUserIdAsync()
        {
            var userId = Guid.NewGuid();
            var userName = "TestUser";
            var filters = new GetPostListFilter { Page = 1, PageSize = 10 };
            var posts = new List<Post> { new Post { Id = Guid.NewGuid().ToString(), AuthorId = userId.ToString() } };
            var postDtos = new List<PostDto> { new PostDto { Id = Guid.NewGuid(), AuthorId = userId.ToString(), AuthorName = userName } };

            _postRepositoryNoSqlMock.Setup(x => x.GetUsersPersonalPostsByUserIdAsync(userId, filters)).ReturnsAsync((posts, posts.Count));

            var result = await _postingService.GetPaginatedDescOrderedUserPostsByUserIdAsync(userId, userName, filters);

            _postRepositoryNoSqlMock.Verify(x => x.GetUsersPersonalPostsByUserIdAsync(userId, filters), Times.Once());

            Assert.NotNull(result);
            Assert.Equal(posts.Count, result.TotalPostsStored);
            Assert.Equal(userName, result.Posts[0].AuthorName);
        }

        [Fact]
        public async Task GetPaginatedDescOrderedUserPostsByUserIdAsync_Failure()
        {
            var userId = Guid.NewGuid();
            var userName = "TestUser";
            var filters = new GetPostListFilter { Page = 1, PageSize = 10 };

            _postRepositoryNoSqlMock.Setup(repo => repo.GetUsersPersonalPostsByUserIdAsync(userId, filters)).ReturnsAsync((null, 0));

            var result = await _postingService.GetPaginatedDescOrderedUserPostsByUserIdAsync(userId, userName, filters);

            _postRepositoryNoSqlMock.Verify(x => x.GetUsersPersonalPostsByUserIdAsync(userId, filters), Times.Once());

            Assert.NotNull(result);
            Assert.Empty(result.Posts);
            Assert.Equal(0, result.TotalPostsStored);
        }
    }
}

