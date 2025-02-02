namespace WFHSocial.Api.Domain.Users.DTOs.UserProfileModels
{
    public class UserProfileUpdateResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
