using Booking.Domain.Models;
using System.Collections.Generic;

namespace Booking.Domain.Abstractions;

public interface IReservationRepository
{
    IReadOnlyList<Reservation> GetReservations(int screeningId);
    int? CreateReservation(int screeningId, int seatId);
    IReadOnlyList<int>? CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests);
    bool CancelReservation(int reservationId);
}