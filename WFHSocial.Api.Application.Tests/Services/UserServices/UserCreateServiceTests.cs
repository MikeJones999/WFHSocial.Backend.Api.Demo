using WFHSocial.Api.Application.Services.UserServices;
using WFHSocial.Api.Domain.Users.DTOs.AuthModels;

namespace WFHSocial.Api.Application.Tests.Services.UserServices
{
    public class UserCreateServiceTests
    {    
        [Theory]
        [InlineData("", "myDisplayName")]
        [InlineData("myDisplayName", "myDisplayName")]
        public void Test_Success_CreateNewUser_UserNameAsDisplayNameWHenNoDisplayNameProvided(string displayName, string expectedName)
        {
            var userRegister = new UserRegister
            {
                UserName = expectedName,
                DisplayName = displayName,
                Email = "testuser@example.com",
                IsRecruiter = true
            };

            var result = UserCreateService.CreateNewUser(userRegister);

            Assert.NotNull(result);
            Assert.Equal(userRegister.UserName, result.UserName);
            Assert.Equal(expectedName, result.DisplayName);
            Assert.Equal(userRegister.Email, result.Email);
            Assert.Equal(userRegister.IsRecruiter, result.IsRecruiter);
        }
    }
}
