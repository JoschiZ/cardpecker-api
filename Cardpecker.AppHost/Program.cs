using Cardpecker.ServiceDefaults;
using MetricsApp.AppHost.OpenTelemetryCollector;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddPostgres(AspireConstants.Databases.Server)
    .WithDataVolume();

var db = sqlServer.AddDatabase(AspireConstants.Databases.Database);



var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
    .WithBindMount("../OpenTelemetry/prometheus", "/etc/prometheus", isReadOnly: true)
    .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(targetPort: 9090, name: "http");



var loki = builder.AddContainer("loki", "grafana/loki")
    .WithBindMount("../OpenTelemetry/loki/config", "/etc/loki", isReadOnly: true)
    .WithHttpEndpoint(targetPort: 3100, name: "http");

var tempo = builder.AddContainer("tempo", "grafana/tempo")
    .WithBindMount("../OpenTelemetry/tempo/config", "/etc/tempo", isReadOnly: true)
    .WithArgs("-config.file=/etc/tempo/tempo.yaml")
    .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/write")
    .WithHttpEndpoint(targetPort: 4317 , name: "http")
    .WithHttpEndpoint(targetPort: 3200, name: "import")
    .WithContainerName("tempo");

tempo.WithEnvironment("TEMPO_ENDPOINT", $"{tempo.GetEndpoint("http")}");

var grafana = builder.AddContainer("grafana", "grafana/grafana")
        .WithBindMount("../OpenTelemetry/grafana/config", "/etc/grafana", isReadOnly: true)
        .WithBindMount("../OpenTelemetry/grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
        .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
        .WithEnvironment("LOKI_ENDPOINT", $"{loki.GetEndpoint("http")}")
        .WithEnvironment("TEMPO_ENDPOINT", $"{tempo.GetEndpoint("import")}")
        .WithHttpEndpoint(targetPort: 3000, name: "http");

builder
    .AddOpenTelemetryCollector("otelcollector", "../OpenTelemetry/otelcollector/config.yaml")
    .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp")
    .WithEnvironment("LOKI_ENDPOINT", $"{loki.GetEndpoint("http")}/otlp")
    .WithEnvironment("TEMPO_ENDPOINT", $"{tempo.GetEndpoint("http")}");



var api = builder
    .AddProject<Cardpecker_Api>(AspireConstants.Api.ApiServer)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
    .WithReference(db)
    .WaitFor(db);


builder
    .AddDockerComposeEnvironment("compose")
    .ConfigureComposeFile(x =>
    {
        foreach (var (name, service) in x.Services)
        {
            service.Restart = "unless-stopped";
        }
        
        var apiService = x.Services[AspireConstants.Api.ApiServer];
        apiService.Networks.Add("otel");
    });

builder.Build().Run();