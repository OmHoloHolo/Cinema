using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public interface ICreateReservationHandler
{
    Task<IReadOnlyList<Reservation.Existing>> Handle(int screeningId, int? seatId);
}
