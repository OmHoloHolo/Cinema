using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Domain.Services;

public class SeatService(IShowProvider showProvider, IReservationRepository reservationRepository) : ISeatService
{
    public async Task<IReadOnlyList<Seat>> GetAvailableSeats(int screeningId)
    {
        var allSeats = await showProvider.GetSeats(screeningId);
        var reservedSeatIds = reservationRepository
            .GetReservations(screeningId)
            .Select(r => r.SeatId)
            .ToList();
        return allSeats.ExceptBy(reservedSeatIds, seat => seat.Id).ToList();
    }
}