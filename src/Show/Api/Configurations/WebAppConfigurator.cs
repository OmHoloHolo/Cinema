using Microsoft.AspNetCore.Builder;
using Show.Api.Mappers;
using Show.Domain.Abstractions;

namespace Show.Api.Configurations;

public static class WebAppConfigurator
{
    public static void ConfigureRoutes(this WebApplication app)
    {
        app.MapGet("/screenings", (IShowService showService) => showService.GetScreenings().ToResponse());
        app.MapGet("/screenings/{screeningId}/seats", (IShowService showService, int screeningId) => showService.GetSeats(screeningId).ToResponse());
    }
}