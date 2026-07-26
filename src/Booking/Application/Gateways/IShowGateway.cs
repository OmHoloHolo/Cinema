using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.Models;

namespace Booking.Application.Gateways;

public interface IShowGateway
{
    Task<IReadOnlyList<SeatSlot>> GetSeatSlots(int screeningId);
}