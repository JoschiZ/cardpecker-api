using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Cardpecker.ServiceDefaults;

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var otelOptions = builder.Configuration.GetSection(OtelOptions.Section).Get<OtelOptions>();
    
        
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });



        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (otelOptions is not null)
                {
                    metrics.AddOtlpExporter((options) =>
                    {
                        options.ConfigureExporter(otelOptions);
                    });
                    metrics.ConfigureResource(resourceBuilder =>
                    {
                        resourceBuilder.ConfigureResources(otelOptions);
                    });
                }

            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(tracing =>
                        // Don't trace requests to the health endpoint to avoid filling the dashboard with noise
                        tracing.Filter = httpContext =>
                            !(httpContext.Request.Path.StartsWithSegments(HealthEndpointPath)
                              || httpContext.Request.Path.StartsWithSegments(AlivenessEndpointPath))
                    )
                    .AddHttpClientInstrumentation();
                
                if (otelOptions is not null)
                {
                    tracing.AddOtlpExporter((options) =>
                    {
                        options.ConfigureExporter(otelOptions);
                    });
                    tracing.ConfigureResource(resourceBuilder =>
                    {
                        resourceBuilder.ConfigureResources(otelOptions);
                    });
                }

            })
            .WithLogging(logging =>
            {
                if (otelOptions is not null)
                {
                    logging.AddOtlpExporter((options) =>
                    {
                        options.ConfigureExporter(otelOptions);
                    });
                    logging.ConfigureResource(resourceBuilder =>
                    {
                        resourceBuilder.ConfigureResources(otelOptions);
                    });
                }
            });

        if (otelOptions is null)
        {
            builder.AddOpenTelemetryExporters();
        }
        

        return builder;
    }

    private static OtlpExporterOptions ConfigureExporter(this OtlpExporterOptions options, OtelOptions otelOptions)
    {
        options.Endpoint = otelOptions.CollectorUri;
        options.Protocol = OtlpExportProtocol.Grpc;
        
        return options;
    }

    private static ResourceBuilder ConfigureResources(this ResourceBuilder builder, OtelOptions otelOptions)
    {
        builder.AddAttributes([
            new KeyValuePair<string, object>("service.name", otelOptions.ServiceName),
            new KeyValuePair<string, object>("service.instance.id", otelOptions.InstanceName),
        ]);
        
        return builder; 
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);


        
        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(HealthEndpointPath);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
