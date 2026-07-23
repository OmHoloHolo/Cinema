using Booking.Domain.Models;
using System.Collections.Generic;

namespace Booking.Domain.Abstractions;

public interface IReservationRepository
{
    IReadOnlyList<Reservation> GetReservations(int screeningId);
    int? CreateReservation(int screeningId, int seatId);
    bool CancelReservation(int reservationId);
    IReadOnlyList<Seat> GetReservedSeats(int screeningId);
}