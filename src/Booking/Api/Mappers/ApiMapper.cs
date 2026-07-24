using System.Collections.Generic;
using System.Linq;
using Booking.Api.Requests;
using Booking.Api.Responses;
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

    public static MultipleReservationsResponse ToResponse(this IEnumerable<Reservation> reservations) => new(
        Reservations: reservations
            .Select(reservation => new MultipleReservationsResponse.Reservation(
                Id: reservation.Id,
                ScreeningId: reservation.ScreeningId,
                SeatId: reservation.SeatId))
            .ToList());

    public static AvailableSeatsResponse ToResponse(this IEnumerable<Seat> seats) => new(
        Seats: seats
            .Select(seat => new AvailableSeatsResponse.Seat(
                Id: seat.Id,
                Row: seat.Row,
                Number: seat.Number))
            .ToList());

    public static ReservationResponse ToResponse(this Reservation reservation) => new(
        Id: reservation.Id,
        ScreeningId: reservation.ScreeningId,
        SeatId: reservation.SeatId);
}