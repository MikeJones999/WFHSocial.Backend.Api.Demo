namespace WFHSocial.Api.Application.Interfaces.DomainEvents
{
    public interface IProfileImageFileUploadedEvents
    {
        Task ResizeImageAsync(string filePath);
    }
}
