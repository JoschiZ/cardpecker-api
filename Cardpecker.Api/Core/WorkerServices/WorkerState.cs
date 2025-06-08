using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cardpecker.Api.Core.WorkerServices;

public class WorkerState
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? LastRun { get; set; }
}

internal class WorkerStateConfig : IEntityTypeConfiguration<WorkerState>
{
    public void Configure(EntityTypeBuilder<WorkerState> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}