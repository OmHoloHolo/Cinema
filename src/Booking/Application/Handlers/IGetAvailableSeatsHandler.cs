using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public interface IGetAvailableSeatsHandler
{
    Task<IReadOnlyList<Seat>> Handle(int requestedScreeningId);
}
