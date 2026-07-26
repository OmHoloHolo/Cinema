using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Api;
using Shared.Api.Models;
using Booking.Api.Mappers;
using Booking.Api.Requests;
using Booking.Application.Handlers;

namespace Booking.Api.Configurations;

public static class WebAppConfigurator
{
    public static void ConfigureRoutes(this WebApplication app, ILogger logger)
    {
        app.MapGet("/auth/token", () => Results.Ok(new TokenResponse(AuthenticationUtils.GenerateToken(app.Configuration))))
            .WithSummary("Get authentication token")
            .WithDescription("Get the authentication token to paste in the Authorize section above");

        app.MapGet(
            "/screenings/{screeningId}/available-seats", 
            (IGetAvailableSeatsHandler handler, int screeningId) => 
                HandleException(logger, async () =>
                {
                    var availableSeats = await handler.Handle(screeningId);
                    return Results.Ok(availableSeats.ToResponse());
                }))
            .RequireAuthorization()
            .WithSummary("Get available seats")
            .WithDescription("Get all available seats of a screening");

        app.MapGet(
            "/screenings/{screeningId}/reservations", 
            (IGetReservationsHandler handler, int screeningId) => 
                HandleException(logger, async () =>
                {
                    var reservations = await handler.Handle(screeningId);
                    return Results.Ok(reservations.ToResponse());
                }))
            .RequireAuthorization()
            .WithSummary("Get reservations")
            .WithDescription("Get all existing reservations of a screening");

        app.MapPost(
            "screenings/{screeningId}/reservations",
            (ICreateReservationHandler handler, int screeningId, [FromBody] ReservationCreationRequest request) =>
                HandleException(logger, async () =>
                {
                    var reservation = await handler.Handle(screeningId, request.SeatId);
                    return Results.Created(string.Empty, reservation.ToResponse());
                }))
            .RequireAuthorization()
            .WithSummary("Create reservation")
            .WithDescription("Create the reservation of a screening with the seat requested, if specified, otherwise create the reservation for any available seat");

        app.MapDelete(
            "screenings/{screeningId}/reservations/{reservationId}", 
            (ICancelReservationHandler handler, int screeningId, int reservationId) =>
                HandleException(logger, async () =>
                {
                    await handler.Handle(screeningId, reservationId);
                    return Results.NoContent();
                })
            ).RequireAuthorization()
            .WithSummary("Cancel reservation")
            .WithDescription("Cancel the reservation of a screening");

        app.MapPost(
            "/multiple-reservations",
            (ICreateMultipleReservationsHandler handler, [FromBody] MultipleReservationCreationRequest request) => 
                HandleException(logger, async () =>
                {
                    var reservationRequests = request.ToDomain();
                    var reservations = await handler.Handle(reservationRequests);
                    return Results.Created(string.Empty, reservations.ToResponse());
                }))
            .RequireAuthorization()
            .WithSummary("Create multiple reservations")
            .WithDescription("Create multiple reservations with the possibility to choose different screenings in one request");
    }

    private static async Task<IResult> HandleException(ILogger logger, Func<Task<IResult>> process)
    {
        try
        {
            return await process();
        }
        catch (Exception ex) when (ex is InvalidOperationException or InvalidDataException)
        {
            logger.LogWarning(message: $"Handled exception: {ex.Message}", exception: ex);
            return Results.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(message: "Unhandled exception", exception: ex);
            throw;
        }
    }
}