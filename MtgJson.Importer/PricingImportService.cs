using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MtgJson.Importer.Context;
using MtgJson.Importer.Pricing;
using SharpCompress.Compressors.Xz;

namespace MtgJson.Importer;

public class PricingImportService
{
    private readonly HttpClient _httpClient;

    public PricingImportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PricingInfo>> GetPricingInfoAsync(CancellationToken cancellationToken = default)
    {
        var pricingFileTask = GetPricingDataAsync(cancellationToken);
        var idMappingsTask = GetMappingsAsync(cancellationToken);
        await Task.WhenAll(pricingFileTask, idMappingsTask).ConfigureAwait(false);

        if (pricingFileTask.Result is null)
        {
            throw new Exception("Could not fetch or parse pricing file");
        }
        
        return GetPricingInfos(idMappingsTask.Result, pricingFileTask.Result);
    }

    private IEnumerable<PricingInfo> GetPricingInfos(IdMapping[] idMappings, PricingFile pricingFile)
    {
        foreach (var (mtgJsonId, scryfallId) in idMappings)
        {
            if (!pricingFile.Data.TryGetValue(mtgJsonId, out var priceData)) continue;
            
            foreach (var (provider, priceList) in priceData.MagicOnline)
            {
                if (priceList.Retail is null)
                {
                    continue;
                }
                var etchedPrice = priceList.Retail.Etched?.FirstOrDefault();
                if (etchedPrice is not null)
                {
                    yield return new PricingInfo(
                        scryfallId, 
                        provider,
                        priceList.Currency,
                        etchedPrice.Value.Value,
                        true,
                        "etched",
                        etchedPrice.Value.Key);
                }
                    
                var foilPrice = priceList.Retail.Foil?.FirstOrDefault();
                if (foilPrice is not null)
                {
                    yield return new PricingInfo(
                        scryfallId, 
                        provider,
                        priceList.Currency,
                        foilPrice.Value.Value,
                        true,
                        "foil",
                        foilPrice.Value.Key);
                }
                    
                var normal = priceList.Retail.Normal?.FirstOrDefault();
                if (normal is not null)
                {
                    yield return new PricingInfo(
                        scryfallId, 
                        provider,
                        priceList.Currency,
                        normal.Value.Value,
                        true,
                        "normal",
                        normal.Value.Key);
                }
                    
            }
                
            foreach (var (provider, priceList) in priceData.PaperMagic)
            {
                if (priceList.Retail is null)
                {
                    continue;
                }
                var etchedPrice = priceList.Retail.Etched?.FirstOrDefault();
                if (etchedPrice is not null)
                {
                    yield return new PricingInfo(
                        scryfallId, 
                        provider,
                        priceList.Currency,
                        etchedPrice.Value.Value,
                        false,
                        "etched",
                        etchedPrice.Value.Key);
                }
                    
                var foilPrice = priceList.Retail.Foil?.FirstOrDefault();
                if (foilPrice is not null)
                {
                    yield return new PricingInfo(
                        scryfallId, 
                        provider,
                        priceList.Currency,
                        foilPrice.Value.Value,
                        false,
                        "foil",
                        foilPrice.Value.Key);
                }
                    
                var normal = priceList.Retail.Normal?.FirstOrDefault();
                if (normal is not null)
                {
                    yield return new PricingInfo(
                        scryfallId, 
                        provider,
                        priceList.Currency,
                        normal.Value.Value,
                        false,
                        "normal",
                        normal.Value.Key);
                }
                    
            }
        }

    }


    private async Task<PricingFile?> GetPricingDataAsync(CancellationToken cancellationToken)
    {
        var xsCompressed = await _httpClient.GetStreamAsync("https://mtgjson.com/api/v5/AllPricesToday.json.xz", cancellationToken).ConfigureAwait(false);
        await using var compressed = xsCompressed.ConfigureAwait(false);
        var tempMs = new MemoryStream();
        await using var ms = tempMs.ConfigureAwait(false);
        await xsCompressed.CopyToAsync(tempMs, cancellationToken).ConfigureAwait(false);
        tempMs.Seek(0, SeekOrigin.Begin);
        var xsStream = new XZStream(tempMs);
        await using var stream = xsStream.ConfigureAwait(false);
        var pricingFile = await JsonSerializer.DeserializeAsync<PricingFile>(xsStream, cancellationToken: cancellationToken).ConfigureAwait(false);

        return pricingFile;
    }

    private async Task<IdMapping[]> GetMappingsAsync(CancellationToken cancellationToken)
    {
        var fs = File.Open("allPrintings.sqlite", FileMode.Create);
        await using (fs.ConfigureAwait(false))
        {
            var xsCompressedAllPrintings = await _httpClient.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.sqlite.xz", cancellationToken).ConfigureAwait(false);
            await using var compressedAllPrintings = xsCompressedAllPrintings.ConfigureAwait(false);
            var tempMs = new MemoryStream();
            await using var ms = tempMs.ConfigureAwait(false);
            await xsCompressedAllPrintings.CopyToAsync(tempMs, cancellationToken).ConfigureAwait(false);
            tempMs.Seek(0, SeekOrigin.Begin);
            var xsStream = new XZStream(tempMs);
            await using var stream = xsStream.ConfigureAwait(false);
            await xsStream.CopyToAsync(fs, cancellationToken).ConfigureAwait(false);
        }

        var contextOptions = new DbContextOptionsBuilder<MtgJsonContext>()
            .UseSqlite("Data Source=allPrintings.sqlite")
            .Options;

        var context = new MtgJsonContext(contextOptions);
        await using var _ = context.ConfigureAwait(false);


        var idMappings = await context
            .CardIdentifiers
            .AsNoTracking()
            .Where(x => x.ScryfallId != null)
            .Select(x => new IdMapping(x.Uuid, x.ScryfallId!.Value))
            .ToArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return idMappings;
    }

}