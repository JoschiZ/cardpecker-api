using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Cardpecker.Api;

public class PlaceholderEndpoint : Endpoint<RequestObj, Ok<ResponseObj>>
{
    public override void Configure()
    {
        Get("hello-world");
        AllowAnonymous();
    }

    public override Task<Ok<ResponseObj>> ExecuteAsync(RequestObj req, CancellationToken ct)
    {
        return Task.FromResult(TypedResults.Ok(new ResponseObj(req.Hallo)));
    }
}
public record RequestObj(string Hallo);
public record ResponseObj(string Hallo);