using System;
using System.IO;
using System.Threading.Tasks;
using Booking.Api.Mappers;
using Booking.Api.Requests;
using Booking.Application.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Configurations;

public static class WebAppConfigurator
{
    public static void ConfigureRoutes(this WebApplication app)
    {
        app.MapGet(
            "/screenings/{screeningId}/available-seats", 
            (IGetAvailableSeatsHandler handler, int screeningId) => 
                HandleException(async () =>
                {
                    var availableSeats = await handler.Handle(screeningId);
                    return Results.Ok(availableSeats.ToResponse());
                }));

        app.MapPost(
            "screenings/{screeningId}/reservations",
            (ICreateReservationHandler handler, int screeningId, [FromBody] ReservationCreationRequest request) =>
                HandleException(async () =>
                {
                    var reservation = await handler.Handle(screeningId, request.SeatId);
                    return Results.Created(string.Empty, reservation.ToResponse());
                }));

        app.MapDelete(
            "screenings/{screeningId}/reservations/{reservationId}", 
            (ICancelReservationHandler handler, int screeningId, int reservationId) =>
                HandleException(async () =>
                {
                    await handler.Handle(screeningId, reservationId);
                    return Results.NoContent();
                }));


        app.MapPost(
            "/multiple-reservations",
            (ICreateReservationsHandler handler, [FromBody] MultipleReservationCreationRequest request) => 
                HandleException(async () =>
                {
                    var reservationRequests = request.ToDomain();
                    var reservations = await handler.Handle(reservationRequests);
                    return Results.Created(string.Empty, reservations.ToResponse());
                }));
    }

    private static Task<IResult> HandleException(Func<Task<IResult>> process)
    {
        try
        {
            return process();
        }
        catch (Exception ex) when (ex is InvalidOperationException or InvalidDataException)
        {
            return Task.FromResult(Results.Conflict(ex.Message));
        }
    }
}