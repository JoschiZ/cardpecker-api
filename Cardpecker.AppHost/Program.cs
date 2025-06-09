using Aspire.Hosting.Docker.Resources.ComposeNodes;
using Cardpecker.ServiceDefaults;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddPostgres(AspireConstants.Databases.Server)
    .WithDataVolume();

var db = sqlServer.AddDatabase(AspireConstants.Databases.Database);

var api = builder
    .AddProject<Cardpecker_Api>(AspireConstants.Api.ApiServer)
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
        
        x.Services[AspireConstants.Api.ApiServer].Networks.Add("otel");
        x.Networks.Add("otel", new Network()
        {
            Name = "otel",
            External = true
        });
    });

builder.Build().Run();