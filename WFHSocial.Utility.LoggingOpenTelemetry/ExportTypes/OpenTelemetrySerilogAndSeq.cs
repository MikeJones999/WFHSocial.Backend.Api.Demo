using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace WFHSocial.Utility.LoggingOpenTelemetry.ExportTypes
{
    public static class OpenTelemetrySerilogAndSeq
    {
        public static IServiceCollection AddOpenTelemetrySerilogAndSeqConnection(IServiceCollection services, OpenTelemetrySettings settings, string environmentName)
        {
            Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .WriteTo.OpenTelemetry(x =>
             {
                 x.Endpoint = settings.Endpoint; //"http://localhost:5341/ingest/otlp/v1/logs";
                 x.Protocol = OtlpProtocol.HttpProtobuf;
                 x.Headers = new Dictionary<string, string>
                 {
                     ["X-Seq-ApiKey"] = settings.SecurityKey //"H2b1dONy3YfyQHmYFhMB"
                 };

                 x.ResourceAttributes = new Dictionary<string, object>()
                 {
                     ["deployment.environment"] = environmentName //builder.Environment.EnvironmentName
                 }; 
             })
             .CreateLogger();

            services.AddSerilog();
            return services;
        }
    }
}
