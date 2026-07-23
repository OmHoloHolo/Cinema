using Booking.Domain.Models;
using System.Collections.Generic;

namespace Booking.Domain.Abstractions;

public interface IReservationRepository
{
    IReadOnlyList<Reservation> GetReservations(int screeningId);
    Reservation? CreateReservation(int screeningId, int seatId);
    IReadOnlyList<Reservation>? CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests);
    bool CancelReservation(int reservationId);
}