namespace Cardpecker.Api.Core.WorkerServices;

public static class WorkerServiceExtensions
{
    public static IServiceCollection AddWorkerService<TWorker, TWorkload>(this IServiceCollection services)
        where TWorker : WorkerBase<TWorkload>
        where TWorkload : class, IWorkload
    {
        services.AddScoped<TWorkload>();
        services
            .AddOptions<WorkerOptions<TWorkload>>()
            .ValidateDataAnnotations()
            .BindConfiguration(TWorkload.OptionsSectionName);
        services.AddHostedService<TWorker>();
        return services;

    }
}