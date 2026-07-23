using Booking.Domain.Models;
using System.Collections.Generic;

namespace Booking.Domain.Abstractions;

public interface IBookingService
{
    IReadOnlyList<Seat> GetAvailableSeats(int screeningId);
    int CreateReservation(int screeningId, int seatId);
    int CreateReservation(int screeningId);
    bool CancelReservation(int reservationId);
}
