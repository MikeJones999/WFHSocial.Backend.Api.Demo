using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WFHSocial.Utility.LoggingOpenTelemetry.ExportTypes;

namespace WFHSocial.Utility.LoggingOpenTelemetry
{
    public static class Startup
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration, string environmentName)
        {
            OpenTelemetrySettings settings = ExtractopenTelemetrySettingsFromConfig(services, configuration);
            return IdentifyAndAddOpenTelemetryConnection(services, settings, environmentName);
        }

        public static OpenTelemetrySettings ExtractopenTelemetrySettingsFromConfig(IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection? appSettingSection = configuration.GetSection("OpenTelemetrySettings");
            if (appSettingSection == null)
            {
                //throw 
            }
            services.Configure<OpenTelemetrySettings>(appSettingSection);
            OpenTelemetrySettings appsettings = appSettingSection.Get<OpenTelemetrySettings>();
            if (appSettingSection == null)
            {
                //throw
            }
            return appsettings;
        }

        public static IServiceCollection IdentifyAndAddOpenTelemetryConnection(IServiceCollection services, OpenTelemetrySettings settings, string environmentName)
        {
            if (settings.EndpointType.ToLower().Equals("default-seq"))
            {
                return OpenTelemetryDefaultAndSeq.AddOpenTelemetryDefaultAndSeqConnection(services, settings, environmentName);
            }

            if (settings.EndpointType.ToLower().Equals("serilog-seq"))
            {
                return OpenTelemetrySerilogAndSeq.AddOpenTelemetrySerilogAndSeqConnection(services, settings, environmentName);
            }

            //should throw is nothing found

            return services;
        }    
    }
}
