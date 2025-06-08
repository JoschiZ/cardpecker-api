using Cardpecker.Api.Core.WorkerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cardpecker.Api;

public class PeckerContext : DbContext
{
    public PeckerContext(DbContextOptions<PeckerContext> options) : base(options){}
    
    public DbSet<MagicCardInfo> MagicCards { get; set; }
    public DbSet<MagicCardPricingPoint> MagicCardPricingPoints { get; set; }
    public DbSet<WorkerState> WorkerStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PeckerContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

public class MagicCardInfo
{
    public required Guid ScryfallId { get; set; }
    public List<MagicCardPricingPoint> Pricings { get; set; } = [];
}

public class MagicCardPricingPoint
{
    public required Guid ScryfallId { get; set; }
    public required string PricingProvider { get; set; }
    public required string Currency { get; set; }
    public required decimal Price { get; set; }
    public required bool IsMagicOnline { get; set; }
    public required string PrintingVersion { get; set; }
    public required DateOnly PriceDate { get; set; }
}

internal class MagicCardInfoConfiguration : IEntityTypeConfiguration<MagicCardInfo>
{
    public const string Schema = "Magic";
    public const string TableName = "CardInfos";
    public const string FullTableName = Schema + "." + TableName;
    public void Configure(EntityTypeBuilder<MagicCardInfo> builder)
    {
        builder.HasKey(x => x.ScryfallId);
        builder.ToTable(TableName, Schema);
        builder.Property(x => x.ScryfallId).ValueGeneratedNever();
        
        builder.HasMany(x => x.Pricings).WithOne().HasForeignKey(x => x.ScryfallId);
    }
}

internal class MagicCardPricingPointConfiguration : IEntityTypeConfiguration<MagicCardPricingPoint>
{
    public const string Schema = "Magic";
    public const string TableName = "PricingPoints";
    public const string FullTableName = Schema + "." + TableName;
    public void Configure(EntityTypeBuilder<MagicCardPricingPoint> builder)
    {
        
        builder.HasKey(x => new{x.ScryfallId, x.PricingProvider, x.PrintingVersion, x.IsMagicOnline, x.Currency});
        builder.ToTable(TableName, Schema);
        builder.HasIndex(x => x.ScryfallId);
        builder.Property(x => x.ScryfallId).ValueGeneratedNever();
        builder.Property(x => x.PricingProvider).HasMaxLength(40);
        builder.Property(x => x.Currency).HasMaxLength(10);
        builder.Property(x => x.PrintingVersion).HasMaxLength(10);

    }
}