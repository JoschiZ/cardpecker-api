using Cardpecker.Api;
using Cardpecker.ServiceDefaults;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;


services
    .AddFastEndpoints()
    .SwaggerDocument(x =>
    {
        x.ShortSchemaNames = true;
        x.DocumentSettings = settings =>
        {
            settings.MarkNonNullablePropsAsRequired();
        };
        x.UseOneOfForPolymorphism = true;
    });


services.AddDbContextPool<PeckerContext>(x =>
{
    x.UseNpgsql(config.GetConnectionString(AspireConstants.Databases.Database));
    x.UseAsyncSeeding(async (context, migrationOperation, cancellationToken) =>
    {
        if (!builder.Environment.IsDevelopment())
        {
            return;
        }
        
        if (!migrationOperation)
        {
            return;
        }
    });
});
builder.EnrichNpgsqlDbContext<PeckerContext>();




var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(config => config.Path = "/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.Run();
