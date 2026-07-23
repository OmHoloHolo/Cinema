using Microsoft.Extensions.DependencyInjection;

namespace Booking.Migration;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrationServices(this IServiceCollection services)
    {
        services.AddScoped<MigrationService>();

        return services;
    }
}