using Cardpecker.Api.Core.WorkerServices;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MtgJson.Importer;

namespace Cardpecker.Api.MagicTheGathering.Pricing;

public class PricingWorker : WorkerBase<ImportPricingWorkload>
{
    public PricingWorker(IServiceScopeFactory scopeFactory, IOptions<WorkerOptions<ImportPricingWorkload>> options, ILogger<WorkerBase<ImportPricingWorkload>> logger) : base(scopeFactory, options, logger)
    {
    }
}

public class ImportPricingWorkload : IWorkload
{
    private readonly PeckerContext _context;
    private readonly PricingImportService _importService;

    public ImportPricingWorkload(PeckerContext context, PricingImportService importService)
    {
        _context = context;
        _importService = importService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!await CheckImportRequiredAsync(cancellationToken))
        {
            return;
        }

        var dataTask = _importService.GetPricingInfoAsync(cancellationToken);



        var pricingData = (await dataTask).ToArray();
        var cardData = pricingData
            .GroupBy(x => x.ScryfallId, (scryfallId, infos) => new MagicCardInfo()
            {
                ScryfallId = scryfallId
            })
            .Select(x => x);
        await _context.BulkInsertOrUpdateAsync(cardData, cancellationToken: cancellationToken);

        var pricings = pricingData
            .Select(info => new MagicCardPricingPoint
            {
                ScryfallId = info.ScryfallId,
                PricingProvider = info.Marketplace.ToString(),
                Currency = info.Currency,
                Price = info.Price,
                IsMagicOnline = info.IsMtgOnline,
                PrintingVersion = info.CardVersion,
                PriceDate = info.Date
            })
            .DistinctBy(x => new { x.ScryfallId, x.PricingProvider, x.PrintingVersion, x.IsMagicOnline, x.Currency });
        
        await _context.TruncateAsync<MagicCardPricingPoint>(cancellationToken: cancellationToken);
        await _context.BulkInsertAsync(pricings, cancellationToken: cancellationToken);

    }

    public static string OptionsSectionName { get; } = "PricingWorker";

    private async Task<bool> CheckImportRequiredAsync(CancellationToken cancellationToken)
    {
        var firstPrice = await _context.MagicCardPricingPoints.FirstOrDefaultAsync(cancellationToken);

        if (firstPrice is null)
        {
            return true;
        }

        if (firstPrice.PriceDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return true;
        }
        
        return false;
    }
}