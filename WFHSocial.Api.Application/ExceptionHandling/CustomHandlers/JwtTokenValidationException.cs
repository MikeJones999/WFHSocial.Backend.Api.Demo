using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace WFHSocial.Api.Application.ExceptionHandling.CustomHandlers
{
    [ExcludeFromCodeCoverage]
    public class JwtTokenValidationException : BaseException
    {
        public JwtTokenValidationException(string message = "") : base($"Validation of Jwt failed. {message}. Unable to continue", HttpStatusCode.Unauthorized)
        {
        }
    }
}
