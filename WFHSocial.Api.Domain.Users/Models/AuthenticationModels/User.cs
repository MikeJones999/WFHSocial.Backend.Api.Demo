namespace WFHSocial.Api.Domain.Users.Models.AuthenticationModels
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedDateUtc { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? DateModifiedUtc { get; set; }

    }
}
