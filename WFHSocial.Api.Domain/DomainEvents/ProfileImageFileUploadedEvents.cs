using WFHSocial.Api.Application.Interfaces.DomainEvents;

namespace WFHSocial.Api.Domain.DomainEvents
{
    public class ProfileImageFileUploadedEvents: IProfileImageFileUploadedEvents
    {
        public Task ResizeImageAsync(string filePath)
        {
            return Task.CompletedTask;
        }
    }
}
