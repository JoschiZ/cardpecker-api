using Cardpecker.ServiceDefaults;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddPostgres(AspireConstants.Databases.Server);

var db = sqlServer.AddDatabase(AspireConstants.Databases.Database);

var api = builder
    .AddProject<Cardpecker_Api>(AspireConstants.Api.ApiServer)
    .WaitFor(db);

builder.Build().Run();