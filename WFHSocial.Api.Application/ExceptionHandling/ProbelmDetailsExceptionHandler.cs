﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WFHSocial.Api.Application.ExceptionHandling
{
    public class ProbelmDetailsExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Instance = httpContext.Request.Path;
            if (exception is BaseException e)
            {
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = e.Message;
            }
            else
            {
                problemDetails.Title = exception.Message;
            }
            
            problemDetails.Status = httpContext.Response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
