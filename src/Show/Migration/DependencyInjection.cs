
using Microsoft.Extensions.DependencyInjection;

namespace Show.Migration;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrationServices(this IServiceCollection services)
    {
        services.AddScoped<MigrationService>();

        return services;
    }
}