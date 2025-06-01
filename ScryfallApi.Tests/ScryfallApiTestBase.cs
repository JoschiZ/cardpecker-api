using Microsoft.Extensions.DependencyInjection;
using ScryfallApi.Client;

namespace ScryfallApi.Tests;

public abstract class ScryfallApiTestBase
{
    private protected readonly ScryfallApiClient _client;

    protected ScryfallApiTestBase()
    {
        var sc = new ServiceCollection();
        sc.AddScryfallApiClient();
        var provider = sc.BuildServiceProvider();
        _client = provider.GetRequiredService<ScryfallApiClient>();
    }
}