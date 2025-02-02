using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Application.Services.AuthenticationServices;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.DTOs.AuthModels;

namespace WFHSocial.Api.Application.Tests.Services.AuthenticationServices
{
    public class AuthUserServiceTests
    {
        private readonly Mock<ILogger<AuthUserService>> _loggerMock;
        private IConfiguration _configMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IDefaultUserSettingService> _defaultUserSettingsServiceMock;
        private AuthUserService _authUserService;

        public AuthUserServiceTests()
        {
            _loggerMock = new Mock<ILogger<AuthUserService>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            _defaultUserSettingsServiceMock = new Mock<IDefaultUserSettingService>();            
        }

        private void SetupBeforeEachMethod(bool isDefaultConfig = true)
        {
            _configMock = new ConfigurationBuilder()
            .AddInMemoryCollection(AppSettingInMemory(isDefaultConfig))
            .Build();

            _authUserService = new AuthUserService(
                _loggerMock.Object,
                _configMock,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _defaultUserSettingsServiceMock.Object);
        }

        private Dictionary<string, string> AppSettingInMemory(bool isDefaultConfig = true)
        {
            var dict =  new Dictionary<string, string> {
                {"JWT:Key", "RandomStringOfCharacters!MustBeAtLeast64LongInSize||NearEnough?NotSureIfthis2="},
                {"JWT:Issuer",  "https://localhost:7110"},
                {"JWT:Audience",  "https://localhost:7110"},
                {"JWT:RefreshLifetimeInDays", "3"}
            };

            if(!isDefaultConfig)
            {
                dict["JWT:Key"] = null;
            }

            return dict;
        }

        #region Register New User
        [Fact]
        public async Task Test_Success_RegisterNewUserAsync_UserIsValid()
        {
            SetupBeforeEachMethod();
            var userRegister = new UserRegister
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234",
                IsRecruiter = false
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole?)null);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _authUserService.RegisterNewUserAsync(userRegister);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());
            _roleManagerMock.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Once());
            _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once());
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());

            Assert.True(result.IsSuccessful);
            Assert.Null(result.Errors);
        }
           

        [Fact]
        public async Task Test_Success_RegisterNewUserAsync_UserIsValid_Recruiter()
        {
            SetupBeforeEachMethod();
            var userRegister = new UserRegister
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234",
                IsRecruiter = true
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole?)null);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _authUserService.RegisterNewUserAsync(userRegister);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Exactly(1));
            _roleManagerMock.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Once());
            _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(2));
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Exactly(2));

            Assert.True(result.IsSuccessful);
            Assert.Null(result.Errors);
        }

        [Fact]
        public async Task Test_Success_RegisterNewUserAsync_ErrorResponse_createdUserFailed()
        {
            SetupBeforeEachMethod();
            var userRegister = new UserRegister
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234",
                IsRecruiter = false
            };
            IdentityResult? identResult = null;

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identResult);

            var result = await _authUserService.RegisterNewUserAsync(userRegister);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());
            Assert.False(result.IsSuccessful);
            Assert.Contains("Error failed to create an account.", result.Errors);
        }

        [Fact]
        public async Task Test_Success_RegisterNewUserAsync_ErrorResponse_createdUserFailed_NotSuccessfull()
        {
            SetupBeforeEachMethod();
            var userRegister = new UserRegister
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234",
                IsRecruiter = false
            };
            IdentityResult? identResult = IdentityResult.Failed(new IdentityError() { Description = "Failed to insert into Database" });
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identResult);

            var result = await _authUserService.RegisterNewUserAsync(userRegister);
            
            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());
            Assert.False(result.IsSuccessful);
            Assert.Contains("Failed to insert into Database", result.Errors);
        }

        [Fact]
        public async Task Test_Success_RegisterNewUserAsync_UserAlreadyExists()
        {
            SetupBeforeEachMethod();
            var userRegister = new UserRegister
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234",
                IsRecruiter = false
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            var result = await _authUserService.RegisterNewUserAsync(userRegister);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());

            Assert.False(result.IsSuccessful);
            Assert.Contains("User already exists.", result.Errors!);
        }

        [Fact]
        public async Task Test_Success_RegisterNewUserAsync_ErrorResponse_UserRegisterIsNull()
        {
            SetupBeforeEachMethod();

            var result = await _authUserService.RegisterNewUserAsync(null);      

            Assert.False(result.IsSuccessful);
            Assert.Contains("Important Information not present.", result.Errors);
        }
        #endregion

        #region User Login
        [Fact]
        public async Task Test_Success_LoginUserAsync_SuccessfulResponse_WhenCredentialsAreValid()
        {
            SetupBeforeEachMethod();
            var userLogin = new UserLogin
            {
                Email = "testuser@example.com",
                Password = "Test@1234"
            };

            var user = new ApplicationUser 
            { 
                Email = userLogin.Email,
                UserName = "testuser",
                DisplayName = "Test User",
                Id = Guid.NewGuid().ToString()
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>() { "Admin"});

            var result = await _authUserService.LoginUserAsync(userLogin);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Once());

            Assert.True(result.IsAuthorised);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
        }

        [Fact]
        public async Task Test_Failure_LoginUserAsync_ExceptionThrown_CreateSecurityTokenFromTokenKey_Failure_AppSettingsMissing()
        {
            SetupBeforeEachMethod(false);
            var userLogin = new UserLogin
            {
                Email = "testuser@example.com",
                Password = "Test@1234"
            };

            var user = new ApplicationUser
            {
                Email = userLogin.Email,
                UserName = "testuser",
                DisplayName = "Test User",
                Id = Guid.NewGuid().ToString()
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>() { "Admin" });

            _configMock["JWT:Key"] = null;

            await Assert.ThrowsAsync<Exception>(() => _authUserService.LoginUserAsync(userLogin));
        }        

        [Fact]
        public async Task Test_Failure_LoginUserAsync_UnsuccessfulResponse_WhenPasswordCredentialsAreInvalid()
        {
            SetupBeforeEachMethod();
            var userLogin = new UserLogin
            {
                Email = "testuser@example.com",
                Password = "WrongPassword"
            };

            var user = new ApplicationUser
            {
                Email = userLogin.Email,
                UserName = "testuser",
                DisplayName = "Test User",
                Id = Guid.NewGuid().ToString()
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = await _authUserService.LoginUserAsync(userLogin);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());

            Assert.False(result.IsAuthorised);
            Assert.Empty(result.Token);
            Assert.Empty(result.RefreshToken);
        }

        [Fact]
        public async Task Test_Failure_LoginUserAsync_UnsuccessfulResponse_WhenEmailCredentialsAreInvalid()
        {
            SetupBeforeEachMethod();
            var userLogin = new UserLogin
            {
                Email = "testuser@example.com",
                Password = "WrongPassword"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var result = await _authUserService.LoginUserAsync(userLogin);

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once());
            Assert.False(result.IsAuthorised);
            Assert.Empty(result.Token);
            Assert.Empty(result.RefreshToken);
        }
        #endregion

        #region User Refresh Token    

        [Theory]
        [InlineData(-35)]
        [InlineData(+35)]
        public async Task Test_Successfull_RefreshTokenAsync_SuccessfulResponse_WhenRefreshTokenTimeIsCompared(int days)
        {
            SetupBeforeEachMethod();
            var userId = Guid.NewGuid();
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "valid_refresh_token"
            };
           
            var user = new ApplicationUser
            {
                Id = userId.ToString(),
                RefreshToken = "valid_refresh_token",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(days),
                DisplayName = "Test User",
                Email = "testuser@example.com",
                UserName = "testuser"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>() { "Admin" });

            var result = await _authUserService.RefreshTokenAsync(userId, refreshTokenRequest);

            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once());
            _userManagerMock.Verify(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Once());

            Assert.True(result.IsAuthorised);
            if(days < 35)
            {
                Assert.Equal(result.RefreshToken, user.RefreshToken);
            }
            else
            {
                Assert.Equal("valid_refresh_token", result.RefreshToken);
            }
        }
     

        [Fact]
        public async Task Test_Successfull_RefreshTokenAsync_UnsuccessfulResponse_WhenRefreshTokenIsInvalid()
        {
            SetupBeforeEachMethod();
            var userId = Guid.NewGuid();
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "invalid_refresh_token"
            };

            var user = new ApplicationUser
            {
                Id = userId.ToString(),
                RefreshToken = "valid_refresh_token",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _authUserService.RefreshTokenAsync(userId, refreshTokenRequest);

            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once());
            Assert.False(result.IsAuthorised);
            Assert.Empty(result.Token);
            Assert.Empty(result.RefreshToken);
        }

        [Fact]
        public async Task Test_Failure_RefreshTokenAsync_RefreshTokenNotCorrect()
        {
            SetupBeforeEachMethod();
            Guid userId = Guid.NewGuid();
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "not_valid_refresh_token"
            };

            var user = new ApplicationUser
            {
                Id = userId.ToString(),
                RefreshToken = "valid_refresh_token",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _authUserService.RefreshTokenAsync(userId, refreshTokenRequest);

            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once());
            Assert.False(result.IsAuthorised);
            Assert.Empty(result.Token);
            Assert.Empty(result.RefreshToken);
        }

        [Fact]
        public async Task Test_Failure_RefreshTokenAsync_WhenUserIsNull()
        {
            SetupBeforeEachMethod();
            Guid userId = Guid.NewGuid();
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "valid_refresh_token"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser) null);

            var result = await _authUserService.RefreshTokenAsync(userId, refreshTokenRequest);
            
            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once());


            Assert.False(result.IsAuthorised);
            Assert.Empty(result.Token);
            Assert.Empty(result.RefreshToken);
        }

        #endregion
    }
}