﻿using ScryfallApi.Client.Models;
using System.Net;
using static ScryfallApi.Client.Models.SearchOptions;

namespace ScryfallApi.Client.Apis;

///<inheritdoc cref="ICards"/>
public class Cards : ICards
{
    private readonly BaseRestService _restService;

    internal Cards(BaseRestService restService)
    {
        _restService = restService;
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public Task<ScryfallResult<Card>> GetRandom() => _restService.GetAsync<Card>($"/cards/random", false);

    public Task<ScryfallResult<ResultList<Card>>> Search(string query, int page, CardSort sort) =>
        Search(query, page, new SearchOptions { Sort = sort });

    public Task<ScryfallResult<ResultList<Card>>> Search(string query, int page, SearchOptions options = default)
    {
        if (page < 1) page = 1;

        query = WebUtility.UrlEncode(query);
        return _restService.GetAsync<ResultList<Card>>($"/cards/search?q={query}&page={page}&{options.BuildQueryString()}");
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    public Task<ScryfallResult<Card>> Named(string cardName, bool fuzzy, string? setCode = null)
    {
        cardName = WebUtility.UrlEncode(cardName);
        var searchType = fuzzy ? "fuzzy" : "exact";
        var query = setCode is not null ? $"&set={setCode}" : string.Empty;
        return _restService.GetAsync<Card>($"/cards/named?{searchType}={cardName}{query}");
    }
}
