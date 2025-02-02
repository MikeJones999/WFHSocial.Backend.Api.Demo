using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace WFHSocial.Api.Application.ExceptionHandling
{
    [ExcludeFromCodeCoverage]
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
