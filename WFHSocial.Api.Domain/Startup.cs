using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WFHSocial.Api.Domain
{
    public static class Startup
    {
        public static IServiceCollection AddDomainApplication(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IApplicationBuilder UseDomainApplication(this IApplicationBuilder app)
        {           
            return app;
        }
       
    }
}
