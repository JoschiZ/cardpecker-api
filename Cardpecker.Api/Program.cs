using System.Text.Json.Serialization;
using Cardpecker.Api;
using Cardpecker.Api.Core.WorkerServices;
using Cardpecker.Api.MagicTheGathering.Pricing;
using Cardpecker.ServiceDefaults;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using MtgJson.Importer;
using Scalar.AspNetCore;
using ScryfallApi.Client;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
builder.AddServiceDefaults();
services
    .AddFastEndpoints(x =>
    {
        
    })
    .SwaggerDocument(x =>
    {
        x.ShortSchemaNames = true;
        x.DocumentSettings = settings =>
        {
            settings.MarkNonNullablePropsAsRequired();
            settings.SchemaSettings.GenerateEnumMappingDescription = true;
        };
        x.UseOneOfForPolymorphism = true;
    });


services.AddDbContextPool<PeckerContext>(x =>
{
    var connectionString = config.GetConnectionString(AspireConstants.Databases.Database);
    if(builder.Environment.IsDevelopment())
    {
        connectionString += ";Include Error Detail = true";
        x.EnableDetailedErrors();
        x.EnableSensitiveDataLogging();
    }
    
    x.UseNpgsql(connectionString);

});
builder.EnrichNpgsqlDbContext<PeckerContext>();
services.AddScryfallApiClient();

services.AddWorkerService<PricingWorker, ImportPricingWorkload>();
services.AddScoped<PricingImportService>();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints(options =>
{
    options.Errors.UseProblemDetails();
    options.Versioning.Prefix = "v";
    options.Versioning.PrependToRoute = true;
    //options.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(openApiConfig => openApiConfig.Path = "/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var peckerContext = scope.ServiceProvider.GetRequiredService<PeckerContext>();
    await peckerContext.Database.MigrateAsync();
}

app.Run();
