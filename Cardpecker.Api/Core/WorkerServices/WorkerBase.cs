using Cardpecker.Api.Otel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cardpecker.Api.Core.WorkerServices;

/// <summary>
/// The baseclass for a background worker performing periodically scheduled tasks
/// </summary>
/// <typeparam name="TWorkload"></typeparam>
internal class WorkerBase<TWorkload>
    :BackgroundService
    where TWorkload : IWorkload
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<WorkerOptions<TWorkload>> _options;
    private readonly ILogger<WorkerBase<TWorkload>> _logger;
    private static readonly string WorkloadName = typeof(TWorkload).Name;
    private bool _isFirstRun = true;
    
    protected WorkerBase(IServiceScopeFactory scopeFactory, IOptions<WorkerOptions<TWorkload>> options, ILogger<WorkerBase<TWorkload>> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(_options.Value.ExecutionInterval);
        do
        {
            if (!_isFirstRun && _options.Value.DontRunBefore is not null && TimeOnly.FromDateTime(DateTime.UtcNow) < _options.Value.DontRunBefore.Value)
            {
                continue;
            }
            _isFirstRun = false;
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                var context = scope.ServiceProvider.GetRequiredService<PeckerContext>();

                var state = await context
                    .WorkerStates
                    .FirstOrDefaultAsync(x => x.Name == WorkloadName, stoppingToken)
                    .ConfigureAwait(false);

                if (state is null)
                {
                    state = new WorkerState() { Name = WorkloadName };
                    context.WorkerStates.Add(state);
                }

                if (state.LastRun is not null)
                {
                    var nextExecution = state.LastRun.Value.ToUniversalTime().Add(_options.Value.ExecutionInterval);
                    var timeToWait = nextExecution - DateTimeOffset.UtcNow;
                    if (timeToWait > TimeSpan.Zero)
                    {
                        await Task.Delay(timeToWait, stoppingToken).ConfigureAwait(false);
                    }
                }

                using var activity = CardpeckerTracing.Source.StartActivity($"workload.{WorkloadName}");
                using var loggerScope = _logger.BeginScope("Start Workload {workloadName}", WorkloadName);
                
                await DoWork(scope.ServiceProvider, state, stoppingToken);
                state.LastRun = DateTimeOffset.UtcNow;
                await context.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                _logger.LogInformation("Finished Workload");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Workload {workloadName} failed", WorkloadName);
            }

        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    /// <summary>
    /// This method is called every work cycle
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="state"></param>
    /// <param name="stoppingToken"></param>
    protected virtual async Task DoWork(IServiceProvider serviceProvider, WorkerState state, CancellationToken stoppingToken)
    {
        var workload = serviceProvider.GetRequiredService<TWorkload>();
        await workload.StartAsync(stoppingToken);
    }
    
}
