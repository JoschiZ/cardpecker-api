namespace Cardpecker.Api.Core.WorkerServices;

internal static class WorkerServiceExtensions
{
    /// <summary>
    /// Adds a background task
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TWorker"></typeparam>
    /// <typeparam name="TWorkload"></typeparam>
    /// <returns></returns>
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

        services.Configure<HostOptions>(x =>
            x.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);
        return services;

    }
}