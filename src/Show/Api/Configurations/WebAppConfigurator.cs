using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Api;
using Shared.Api.Models;
using Show.Api.Mappers;
using Show.Domain.Abstractions;

namespace Show.Api.Configurations;

public static class WebAppConfigurator
{
    public static void ConfigureRoutes(this WebApplication app)
    {
        app.MapGet("/auth/token", () => Results.Ok(new TokenResponse(AuthenticationUtils.GenerateToken(app.Configuration))));

        app.MapGet("/screenings", (IShowService showService) => showService.GetScreenings().ToResponse())
            .RequireAuthorization();

        app.MapGet("/screenings/{screeningId}/seats", (IShowService showService, int screeningId) => showService.GetSeats(screeningId).ToResponse())
            .RequireAuthorization();
    }
}