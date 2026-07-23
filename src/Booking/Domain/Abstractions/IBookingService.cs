using Booking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions;

public interface IBookingService
{
    Task<IReadOnlyList<Seat>> GetAvailableSeats(int screeningId);
    Reservation? CreateReservation(int screeningId, int seatId);
    Task<Reservation?> CreateReservation(int screeningId);
    Task<IReadOnlyList<Reservation>?> CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests);
    bool CancelReservation(int reservationId);
}
