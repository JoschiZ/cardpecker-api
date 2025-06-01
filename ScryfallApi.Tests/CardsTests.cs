using ScryfallApi.Client.Models;

namespace ScryfallApi.Tests;

public class CardsTests : ScryfallApiTestBase
{
    [Theory]
    [InlineData("Sol Ring")]
    [InlineData("Island")]
    [InlineData("Nicol Bolas, Dragon God")]
    public async Task Named_Should_Find_By_Name(string name)
    {
        var result = await _client.Cards.Named(name, false);
        Assert.True(result.IsSuccess, result.Error?.Details);
    }

    [Fact]
    public async Task Should_Not_Find_By_Name()
    {
        var result = await _client.Cards.Named("", false);
        Assert.True(result.IsError);
    }
    
    [Fact]
    public async Task Should_Get_Random()
    {
        var result = await _client.Cards.GetRandom();
        Assert.True(result.IsSuccess, result.Error?.Details);
    }

    [Theory]
    [InlineData("kw:trample")]
    [InlineData("ci=wubrg")]
    [InlineData("f:commander is:paper t:land")]
    public async Task Should_Query(string queryString)
    {
        var results = await _client.Cards.Search(queryString, 0, SearchOptions.CardSort.Name);
        Assert.True(results.IsSuccess, results.Error?.Details);
        Assert.True(results.Value.Data.Count > 0);
    }
    
}