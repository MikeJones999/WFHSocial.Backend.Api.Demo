using Microsoft.Extensions.DependencyInjection;

namespace WFHSocial.Utility.LoggingOpenTelemetry.ExportTypes
{
    public class OpenTelemetryDefaultAndSeq
    {
        public static IServiceCollection AddOpenTelemetryDefaultAndSeqConnection(IServiceCollection services, OpenTelemetrySettings settings, string environmentName)
        {
            //Open telemetry Logging
            //builder.Logging.ClearProviders(); //clears any existing provier speecified
            //                                  //builder.Logging.AddOpenTelemetry(x => x.AddConsoleExporter()); //console logging
            //builder.Logging.AddOpenTelemetry(x =>
            //{

            //    x.SetResourceBuilder(ResourceBuilder.CreateEmpty()
            //        .AddService("Web API")
            //        .AddAttributes(new Dictionary<string, object>()
            //        {
            //            ["deployment.environment"] = environmentName
            //        }));

            //    x.IncludeScopes = true;
            //    x.IncludeFormattedMessage = true;

            //    x.AddOtlpExporter(a =>
            //    {
            //        a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
            //        a.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            //        a.Headers = "X-Seq-ApiKey=H2b1dONy3YfyQHmYFhMB";
            //    });
            //});

            return services;
        }
    }
}
