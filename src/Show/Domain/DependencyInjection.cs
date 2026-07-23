using Microsoft.Extensions.DependencyInjection;
using Show.Domain.Abstractions;
using Show.Domain.Services;

namespace Show.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IShowService, ShowService>();

        return services;
    }
}