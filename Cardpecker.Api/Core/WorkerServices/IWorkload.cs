namespace Cardpecker.Api.Core.WorkerServices;

public interface IWorkload
{
    public Task StartAsync(CancellationToken cancellationToken);
    public static abstract string OptionsSectionName { get; }
}