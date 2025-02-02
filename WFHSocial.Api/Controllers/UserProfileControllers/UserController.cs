using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFHSocial.Api.Application.Interfaces.Repository;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Domain.Users.DTOs.UserProfileModels;
using WFHSocial.Shared;
using WFHSocial.Shared.FileUploads;

namespace WFHSocial.Api.Controllers.UserProfileControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseAuthController
    {
        private readonly IUserProfileAndSettingsService _userProfileService;
        private readonly IUserRepositoryNoSql _nosqlUserRepo;

        public UserController(ILogger<BaseAuthController> logger, IUserProfileAndSettingsService userProfileService, IUserRepositoryNoSql nosqlUserRepo) : base(logger)
        {
            _userProfileService = userProfileService;
            _nosqlUserRepo = nosqlUserRepo;
        }

        [HttpGet("Profile")]
        public async Task<ActionResult<ResponseDto<UserProfileResponse>>> GetUserProfile(Guid userId)
        {
            ValidateUserPassedInAgainstAuth(userId, nameof(this.GetUserProfile));

            ResponseDto<UserProfileResponse> extResponse = new ResponseDto<UserProfileResponse>();
            extResponse.ResponseData = await _userProfileService.GetUserProfileAndSettingsAsync(userId);
            if (extResponse.ResponseData is null)
            {
                UpdateResponse(extResponse);
                return Unauthorized(extResponse);
            }

            _logger.LogInformation("WHF - Request for User profile completed - userId {UserId} profile returned.", userId);
            return Ok(extResponse);
        }

        [HttpPut("Profile")] 
        public async Task<ActionResult<ResponseDto<UserProfileUpdateResponse>>> UpdateUserProfile(UserProfileUpdateRequest userProfile)  
        {
            if (userProfile == null)
            {
                _logger.LogWarning("WHF - Request for User profile failed - userId Guid was empty. Request {Method}", nameof(this.UpdateUserProfile));
                return BadRequest();
            }
            ValidateUserPassedInAgainstAuth(userProfile.UserId, nameof(this.UpdateUserProfile));

            ResponseDto<UserProfileUpdateResponse> extResponse = new ResponseDto<UserProfileUpdateResponse>();
            UserProfileUpdateResponse response = await _userProfileService.UpdateUserProfileAndSettingsAsync(userProfile);
            if (response is null)
            {
                return BadRequest(extResponse);
            }
            _logger.LogInformation("WHF - Update to User profile completed for userId {UserId}", userProfile.UserId);
            UpdateResponse(extResponse, "Successfully updated profile.", true, response);
            return Ok(extResponse);
        }


        [HttpPost("Profile/image")]   
        public async Task<ActionResult<ResponseDto<UploadResult>>> UpdateProfileImage(List<IFormFile> files)
        {
            IFormFile? file = files.FirstOrDefault();
            ResponseDto<UploadResult> extResponse = new ResponseDto<UploadResult>();
            
            if (file is null)
            {
                UpdateResponse(extResponse, message: "Unable to process request.");
                _logger.LogWarning("WHF - Request for User profile failed - userId Guid was empty. Request {Method}", nameof(this.UpdateUserProfile));
                return BadRequest(extResponse);
            }
            
            try
            {
                extResponse.ResponseData = await _userProfileService.UpdateProfileImageAsync(file, UserId);
                return Ok(extResponse);
            }
            catch (Exception ex)
            {      
                UploadResult uploadResult = new UploadResult();               
                uploadResult.IsSuccessful = false;
                UpdateResponse(extResponse, "Failed to upload Profile image file to storage.", false, uploadResult);             
                _logger.LogWarning("WHF - {errorMessage}. Request {Method}", ex.Message, nameof(this.UpdateProfileImage));
                return BadRequest(extResponse);
            }
        }   
    }

  
}
