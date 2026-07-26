using Microsoft.Extensions.DependencyInjection;
using Booking.Application.Services;
using Booking.Application.Handlers;

namespace Booking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ICreateReservationHandler, CreateReservationHandler>();
        services.AddScoped<ICreateMultipleReservationsHandler, CreateMultipleReservationsHandler>();
        services.AddScoped<IGetAvailableSeatsHandler, GetAvailableSeatsHandler>();
        services.AddScoped<ICancelReservationHandler, CancelReservationHandler>();
        services.AddSingleton<IRandomGenerator, RandomGenerator>();

        return services;
    }
}