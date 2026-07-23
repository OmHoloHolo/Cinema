using Booking.Api.Mappers;
using Booking.Api.Requests;
using Booking.Domain.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Configurations;

public static class WebAppConfigurator
{
    private static readonly IResult ReservationError = Results.Problem(
        title: "Reservations creation in conflict",
        detail: "The chosen seats have already been reserved.",
        statusCode: StatusCodes.Status409Conflict);

    public static void ConfigureRoutes(this WebApplication app)
    {
        app.MapGet("/screenings/{screeningId}/available-seats", async (IBookingService bookingService, int screeningId) => 
            (await bookingService.GetAvailableSeats(screeningId)).ToResponse());

        app.MapPost("/reservations", async (IBookingService bookingService, [FromBody] ReservationCreationRequest request) =>
        {
            var reservation = request.SeatId.HasValue 
                ? bookingService.CreateReservation(request.ScreeningId, request.SeatId.Value)
                : await bookingService.CreateReservation(request.ScreeningId);
            return reservation is null 
                ? ReservationError
                : Results.Ok(reservation.Id);            
        });

        app.MapDelete("/reservations/{reservationId}", (IBookingService bookingService, int reservationId) => 
            bookingService.CancelReservation(reservationId));

        app.MapPost("/multiple-reservations/", async (IBookingService bookingService, [FromBody] MultipleReservationCreationRequest request) =>
        {
            var reservationRequests = request.ToDomain();
            var reservations = await bookingService.CreateReservations(reservationRequests);
            return reservations is null 
                ? ReservationError
                : Results.Ok(reservations.ToResponse());
        });
    }
}