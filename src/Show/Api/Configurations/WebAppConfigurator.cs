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
        app.MapGet("/auth/token", () => Results.Ok(new TokenResponse(AuthenticationUtils.GenerateToken(app.Configuration))))
            .WithSummary("Get authentication token")
            .WithDescription("Get the authentication token to paste in the Authorize section above");

        app.MapGet("/screenings", (IShowService showService) => showService.GetScreenings().ToResponse())
            .RequireAuthorization()
            .WithSummary("Get screenings")
            .WithDescription("Get all existing screenings in program");

        app.MapGet("/screenings/{screeningId}/seats", (IShowService showService, int screeningId) => showService.GetSeats(screeningId).ToResponse())
            .RequireAuthorization()
            .WithSummary("Get seats")
            .WithDescription("Get all existing seats for the screening requested");
    }
}