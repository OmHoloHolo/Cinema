using System;
using System.IO;
using System.Threading.Tasks;
using Booking.Api.Mappers;
using Booking.Api.Requests;
using Booking.Application.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Booking.Api.Configurations;

public static class WebAppConfigurator
{
    public static void ConfigureRoutes(this WebApplication app, ILogger logger)
    {
        app.MapGet(
            "/screenings/{screeningId}/available-seats", 
            (IGetAvailableSeatsHandler handler, int screeningId) => 
                HandleException(logger, async () =>
                {
                    var availableSeats = await handler.Handle(screeningId);
                    return Results.Ok(availableSeats.ToResponse());
                }));

        app.MapPost(
            "screenings/{screeningId}/reservations",
            (ICreateReservationHandler handler, int screeningId, [FromBody] ReservationCreationRequest request) =>
                HandleException(logger, async () =>
                {
                    var reservation = await handler.Handle(screeningId, request.SeatId);
                    return Results.Created(string.Empty, reservation.ToResponse());
                }));

        app.MapDelete(
            "screenings/{screeningId}/reservations/{reservationId}", 
            (ICancelReservationHandler handler, int screeningId, int reservationId) =>
                HandleException(logger, async () =>
                {
                    await handler.Handle(screeningId, reservationId);
                    return Results.NoContent();
                }));


        app.MapPost(
            "/multiple-reservations",
            (ICreateMultipleReservationsHandler handler, [FromBody] MultipleReservationCreationRequest request) => 
                HandleException(logger, async () =>
                {
                    var reservationRequests = request.ToDomain();
                    var reservations = await handler.Handle(reservationRequests);
                    return Results.Created(string.Empty, reservations.ToResponse());
                }));
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