using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public interface IGetReservationsHandler
{
    Task<IReadOnlyList<Reservation.Existing>> Handle(int screeningId);
}
