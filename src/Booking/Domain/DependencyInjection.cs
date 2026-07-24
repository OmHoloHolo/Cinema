using Microsoft.Extensions.DependencyInjection;
using Booking.Domain.Abstractions;
using Booking.Domain.Services;

namespace Booking.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ISeatService, SeatService>();
        services.AddSingleton<IRandomProvider, RandomProvider>();

        return services;
    }
}