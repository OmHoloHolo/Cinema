using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Show.Infrastructure.Persistence;
using Show.Domain.Abstractions;
using Show.Infrastructure.Persistence.Repositories;

namespace Show.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShowDbContext>(options => options.UseSqlite(configuration.GetConnectionString("ShowDb")));
        services.AddScoped<IScreeningRepository, ScreeningRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();

        return services;
    }
}