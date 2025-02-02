using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Domain.Posts.DTOs.PostModels;
using WFHSocial.Shared;

namespace WFHSocial.Api.Controllers.PostsControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : BaseAuthController
    {
        private readonly IPostingService _postingService;

        public PostController(ILogger<PostController> logger, IPostingService postingService) : base(logger)
        {
            _postingService = postingService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<PostDto>>> GetSinglePostByIdAsync(Guid id)
        {
            PostDto? post = await _postingService.GetUsersPersonalPostByIdAsync(UserId, id);
            ResponseDto<PostDto> extResponse = new ResponseDto<PostDto>();

            if(post == null)
            {
                UpdateResponse(extResponse, "Post not found.");
                return BadRequest(extResponse);
            }
            
            extResponse.ResponseData = post;
            return Ok(extResponse);

        }

        [HttpGet("List")]
        public async Task<ActionResult<ResponseDto<PostDto>>> GetAllPostsAsync([FromQuery] GetPostListFilter filters)
        {
            ListPostDto postsAndTotal = await _postingService.GetPaginatedDescOrderedUserPostsByUserIdAsync(UserId, DisplayName, filters);
            ResponseDto<ListPostDto> extResponse = new ResponseDto<ListPostDto>();

            if (postsAndTotal == null)
            {
                UpdateResponse(extResponse,"No posts found matching provided filter and information.");
                return BadRequest(extResponse);
            }

            extResponse.ResponseData = postsAndTotal;
            return Ok(extResponse);
        }
      

        [HttpPost]
        public async Task<ActionResult<ResponseDto<PostCreationResponse>>> CreateNewPostsAsync([FromBody] PostCreationRequest creationRequest)
        {
            Guid? postId = await _postingService.CreateNewUserPostByUserIdAsync(UserId, creationRequest);
            ResponseDto<PostCreationResponse> extResponse = new ResponseDto<PostCreationResponse>();

            if (postId == null)
            {
                UpdateResponse(extResponse, "Unable to create post.");
                return BadRequest(extResponse);
            }

            PostCreationResponse response = new PostCreationResponse()
            {
                Id = postId.Value
            };

            UpdateResponse(extResponse, "Successfully added new post.", true, response);
            return Ok(extResponse);
        }     
    }
}
