using Booking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions;

public interface IBookingService
{
    Task<IReadOnlyList<Seat>> GetAvailableSeats(int screeningId);
    int? CreateReservation(int screeningId, int seatId);
    Task<int?> CreateReservation(int screeningId);
    Task<IReadOnlyList<int>?> CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests);
    bool CancelReservation(int reservationId);
}
