using Booking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions;

public interface IReservationService
{
    Task<Reservation?> CreateReservation(int screeningId, int? seatId);
    Task<IReadOnlyList<Reservation>?> CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests);
    bool CancelReservation(int reservationId);
}
