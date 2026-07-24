using Booking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions;

public interface ISeatService
{
    Task<IReadOnlyList<Seat>> GetAvailableSeats(int screeningId);
}