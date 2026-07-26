using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.Models;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public interface ICreateMultipleReservationsHandler
{
    Task<IReadOnlyList<Reservation.Existing>> Handle(IReadOnlyList<ReservationRequest> reservationRequests);
}