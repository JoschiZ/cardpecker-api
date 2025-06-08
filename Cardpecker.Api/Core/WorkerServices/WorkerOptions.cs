namespace Cardpecker.Api.Core.WorkerServices;

public class WorkerOptions<TWorkload> where TWorkload : IWorkload
{
    public required TimeSpan ExecutionInterval { get; init; } 
}