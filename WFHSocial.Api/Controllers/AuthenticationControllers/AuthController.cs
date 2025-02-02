using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Domain.Users.DTOs.AuthModels;

namespace WFHSocial.Api.Controllers.AuthenticationControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseAuthController
    {
        private readonly IRefreshUserService _authUserService;

        public AuthController(ILogger<AuthController> logger, IRefreshUserService authUserService) : base(logger)
        {
            _authUserService = authUserService;
        }            

        [HttpPost("Refresh")]
        public async Task<ActionResult<RefreshResponse>> RefreshLoggedInUserTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            RefreshResponse response = await _authUserService.RefreshTokenAsync(UserId, refreshTokenRequest);
            if (!response.IsAuthorised)
            {
                _logger.LogWarning(message: "WHF - Failed to refresh token for {Email}. Request {Method}", UserId, nameof(this.RefreshLoggedInUserTokenAsync));
                return Unauthorized(response);
            }
            return Ok(response);
        }
    }
}
