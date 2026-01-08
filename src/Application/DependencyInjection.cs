using Application.Abstractions;
using Application.Currencies.ConvertCurrency;
using Application.Currencies.FindLatestCurrency;
using Application.Currencies.SearchCurrency;
using Application.Pipelines;
using Domain.Currencies;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(QueryLoggingBehavior<,>));
        });

        return services;
    }
}
