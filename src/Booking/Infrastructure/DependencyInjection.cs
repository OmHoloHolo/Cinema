using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Booking.Infrastructure.Persistence;
using Booking.Domain.Abstractions;
using Booking.Infrastructure.Persistence.Repositories;
using Booking.Infrastructure.Communication;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options => options.UseSqlite(configuration.GetConnectionString("BookingDb")));
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IShowProvider, ShowClient>();

        return services;
    }
}