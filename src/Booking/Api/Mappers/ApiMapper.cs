using System.Collections.Generic;
using System.Linq;
using Booking.Api.Requests;
using Booking.Api.Responses;
using Booking.Application.Models;
using Booking.Domain.Models;

namespace Booking.Api.Mappers;

public static class ApiMapper
{
    public static IReadOnlyList<ReservationRequest> ToDomain(this MultipleReservationCreationRequest request) =>
        request.Reservations
            .Select(reservation => new ReservationRequest(
                ScreeningId: reservation.ScreeningId,
                SeatId: reservation.SeatId))
            .ToList();

    public static MultipleReservationsResponse ToResponse(this IEnumerable<Reservation.Existing> existingReservations) => new(
        Reservations: existingReservations
            .Select(existingReservation => new MultipleReservationsResponse.Reservation(
                Id: existingReservation.Id,
                ScreeningId: existingReservation.ScreeningId,
                SeatId: existingReservation.SeatId))
            .ToList());

    public static AvailableSeatsResponse ToResponse(this IEnumerable<Seat> seats) => new(
        Seats: seats
            .Select(seat => new AvailableSeatsResponse.Seat(
                Id: seat.Id,
                Row: seat.Row,
                Number: seat.Number))
            .ToList());

    public static ReservationResponse ToResponse(this Reservation.Existing existingReservation) => new(
        Id: existingReservation.Id,
        ScreeningId: existingReservation.ScreeningId,
        SeatId: existingReservation.SeatId);
}