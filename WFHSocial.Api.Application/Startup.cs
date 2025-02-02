using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WFHSocial.Api.Application.Interfaces.DomainEvents;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Application.Services.AuthenticationServices;
using WFHSocial.Api.Application.Services.PostServices;
using WFHSocial.Api.Application.Services.UserSettingsServices;
using WFHSocial.Api.Domain;
using WFHSocial.Api.Domain.DomainEvents;

namespace WFHSocial.Api.Application
{
    public static class Startup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserProfileAndSettingsService, UserProfileAndSettingsService>();
            services.AddTransient<IRefreshUserService, AuthUserService>();
            services.AddTransient<ILoginAndRegisterUserService, AuthUserService>();            
            services.AddTransient<IDefaultUserSettingService, DefaultUserSettingService>();
            services.AddTransient<IPostingService, PostingService>();
            services.AddTransient<IProfileImageFileUploadedEvents, ProfileImageFileUploadedEvents>();

            AddHangFire(services);
            return services.AddDomainApplication(configuration);
        }

        private static void AddHangFire(IServiceCollection services)
        {

            services.AddHangfire(config => config
          .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseMemoryStorage());
        }

        public static IApplicationBuilder UseClientApplication(this IApplicationBuilder app)
        {
            return app.UseHangfireDashboard();
        }
    
    }
}
