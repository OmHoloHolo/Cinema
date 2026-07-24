using Booking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions;

public interface IShowProvider
{
    Task<IReadOnlyList<Seat>> GetSeats(int screeningId);
}