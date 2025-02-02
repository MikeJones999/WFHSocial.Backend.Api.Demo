using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WFHSocial.Api.Application.Interfaces.DomainEvents;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Application.MappingProfiles;
using WFHSocial.Api.Application.Services.UserSettingsServices;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.DTOs.UserProfileModels;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WFHSocial.Api.Domain.Users.Models;
using WFHSocial.Shared.FileUploads;

namespace WFHSocial.Api.Application.Tests.Services.UserSettingServices
{
    public class UserProfileAndSettingsServiceTests
    {
        private readonly Mock<ILogger<UserProfileAndSettingsService>> _loggerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _actualMapper;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IBlobServices> _blobServicesMock;
        private readonly Mock<IProfileImageFileUploadedEvents> _uploadEventsMock;
        private readonly UserProfileAndSettingsService _userProfileAndSettingsService;

        public UserProfileAndSettingsServiceTests()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfiles>());
            _actualMapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<UserProfileAndSettingsService>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _blobServicesMock = new Mock<IBlobServices>();
            _uploadEventsMock = new Mock<IProfileImageFileUploadedEvents>();
            _userProfileAndSettingsService = new UserProfileAndSettingsService(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _actualMapper,
                _userManagerMock.Object,
                _blobServicesMock.Object,
                _uploadEventsMock.Object
            );
        }

        [Fact]
        public async Task Test_Success_GetUserProfileAndSettingsAsync()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId.ToString(), HasProfilePicture = true };
            var userProfileResponse = new UserProfileResponse { DisplayName = "TestUser", HasProfileImage = true };

            _userRepositoryMock.Setup(x => x.GetUserByUserIdAsync(userId)).ReturnsAsync(user);

            var result = await _userProfileAndSettingsService.GetUserProfileAndSettingsAsync(userId);

            Assert.NotNull(result);
            Assert.True(result.HasProfileImage);
            _userRepositoryMock.Verify(x => x.GetUserByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Test_GetUserProfileAndSettingsAsync_Failure()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(x => x.GetUserByUserIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

            var result = await _userProfileAndSettingsService.GetUserProfileAndSettingsAsync(userId);

            _userRepositoryMock.Verify(x => x.GetUserByUserIdAsync(userId), Times.Once());

            Assert.Null(result);
            _loggerMock.Verify(
          x => x.Log(
              LogLevel.Warning,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((o, t) => string.Equals($"WFH - Unable to find User with the given userId: {userId} in the database. Request method: GetUserProfileAndSettingsAsync", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
          Times.Once);
        }

        [Theory]
        [InlineData(UserSettingTypeView.AllowUseOfProfilePicture, false, true, 1)]
        [InlineData(UserSettingTypeView.Notifications, false, false, 0)]
        public async Task Test_Success_UpdateUserProfileAndSettingsAsync(UserSettingTypeView userSettingtype, bool isOn, bool hasPic, int num)
        {
            var userProfileUpdateRequest = new UserProfileUpdateRequest
            {
                UserId = Guid.NewGuid(),
                DisplayName = "NewDisplayName",
                SettingViews = new List<UserSettingView>
                    {
                        new UserSettingView 
                        { 
                            Id = Guid.NewGuid(), 
                            IsOn = isOn, 
                            SettingType = userSettingtype
                        }
                    }
            };
            var user = new ApplicationUser 
            { 
                Id = userProfileUpdateRequest.UserId.ToString(), 
                DisplayName = "OldDisplayName", 
                UserSettings = new List<UserSetting>(),
                HasProfilePicture = hasPic
            };

            var userProfileUpdateResponse = new UserProfileUpdateResponse();

            _userRepositoryMock.Setup(x => x.GetUserByUserIdAsync(userProfileUpdateRequest.UserId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.SaveAsync()).ReturnsAsync(true);

            var result = await _userProfileAndSettingsService.UpdateUserProfileAndSettingsAsync(userProfileUpdateRequest);

            Assert.NotNull(result);
            Assert.True(result.Success);
            _userRepositoryMock.Verify(x => x.UpdateSettings(It.IsAny<List<UserSetting>>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetUserByUserIdAsync(userProfileUpdateRequest.UserId), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveAsync(), Times.Once);       
            _blobServicesMock.Verify(x => x.DeleteFileAsync(It.IsAny<string>()), Times.Exactly(num));           
        }

        [Fact]
        public async Task Test_Failure_UpdateUserProfileAndSettingsAsync()
        {
            Guid userId = Guid.NewGuid();
            var userProfileUpdateRequest = new UserProfileUpdateRequest
            {
                UserId = userId        
            };

            _userRepositoryMock.Setup(x => x.GetUserByUserIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

            var result = await _userProfileAndSettingsService.UpdateUserProfileAndSettingsAsync(userProfileUpdateRequest);
            
            _userRepositoryMock.Verify(x => x.GetUserByUserIdAsync(userId), Times.Once);
            Assert.NotNull(result);
            Assert.False(result.Success);
            _loggerMock.Verify(
               x => x.Log(
               LogLevel.Warning,
               It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals($"WFH - Unable to find User with the given userId: {userId} in the database. Request method: UpdateUserProfileAndSettingsAsync", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
               Times.Once);
        }

        [Fact]
        public async Task Test_Success_UpdateProfileImageAsync()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            { 
                Id = userId.ToString(), 
                HasProfilePicture = true
            };
            var fileMock = new Mock<IFormFile>();
            var uploadResult = new UploadResult { IsSuccessful = true };

            _userRepositoryMock.Setup(repo => repo.GetUserByUserIdAsync(userId)).ReturnsAsync(user);
            _blobServicesMock.Setup(blob => blob.UploadAsync(fileMock.Object, It.IsAny<string>())).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(repo => repo.SaveAsync()).ReturnsAsync(true);

            var result = await _userProfileAndSettingsService.UpdateProfileImageAsync(fileMock.Object, userId);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            _userRepositoryMock.Verify(repo => repo.GetUserByUserIdAsync(userId), Times.Once);
            _blobServicesMock.Verify(blob => blob.UploadAsync(fileMock.Object, It.IsAny<string>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
            _blobServicesMock.Verify(x => x.DeleteFileAsync(It.IsAny<string>()), Times.Once);

        }

        [Fact]
        public async Task Test_Failure_UpdateProfileImageAsync()
        {
            var userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var uploadResult = new UploadResult { IsSuccessful = false };

            _userRepositoryMock.Setup(repo => repo.GetUserByUserIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

            var result = await _userProfileAndSettingsService.UpdateProfileImageAsync(fileMock.Object, userId);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            _loggerMock.Verify(
            x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals($"Unable to find User with the given userId: {userId} in the database. Request method: UpdateProfileImageAsync", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
        }
    }
}
