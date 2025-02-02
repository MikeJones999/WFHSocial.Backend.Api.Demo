using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace WFHSocial.Api.Application.ExceptionHandling.CustomHandlers
{
    [ExcludeFromCodeCoverage]
    public class UserNotFoundException: BaseException
    {
        public UserNotFoundException(Guid userId, string methodName) : base($"User with id {userId} not found at {methodName}", HttpStatusCode.NotFound)
        {            
        }
    }
}
