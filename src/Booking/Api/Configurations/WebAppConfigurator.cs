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
        title: "Reservation creation in conflict",
        detail: "The chosen seats have already been reserved.",
        statusCode: StatusCodes.Status409Conflict);

    public static void ConfigureRoutes(this WebApplication app)
    {
        app.MapGet("/screenings/{screeningId}/available-seats", async (ISeatService seatService, int screeningId) =>
            (await seatService.GetAvailableSeats(screeningId)).ToResponse());

        app.MapPost(
            "/reservations",
            async (IReservationService reservationService, [FromBody] ReservationCreationRequest request) =>
            {
                var reservation = await reservationService.CreateReservation(request.ScreeningId, request.SeatId);
                return reservation is null
                    ? ReservationError
                    : Results.Ok(reservation.Id);
            });

        app.MapDelete("/reservations/{reservationId}", (IReservationService bookingService, int reservationId) =>
            bookingService.CancelReservation(reservationId));

        app.MapPost(
            "/multiple-reservations", 
            async (IReservationService reservationService, [FromBody] MultipleReservationCreationRequest request) =>
            {
                var reservationRequests = request.ToDomain();
                var reservations = await reservationService.CreateReservations(reservationRequests);
                return reservations is null
                    ? ReservationError
                    : Results.Ok(reservations.ToResponse());
            });
    }
}