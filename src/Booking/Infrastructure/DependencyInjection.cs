using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Booking.Application.Gateways;
using Booking.Application.Repositories;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Persistence.Repositories;
using Booking.Infrastructure.Communication;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options => options.UseSqlite(configuration.GetConnectionString("BookingDb")));
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddHttpClient<AuthenticationHandler>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(configuration.GetRequiredSection("ShowApi:BaseUrl").Get<string>()!);
        });
        services.AddHttpClient<IShowGateway, ShowClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(configuration.GetRequiredSection("ShowApi:BaseUrl").Get<string>()!);
        }).AddHttpMessageHandler<AuthenticationHandler>();

        return services;
    }
}