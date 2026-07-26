using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.Services;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public class GetAvailableSeatsHandler(IReservationService reservationService) : IGetAvailableSeatsHandler
{
    public async Task<IReadOnlyList<Seat>> Handle(int requestedScreeningId)
    {
        var reservationsAggregate = await reservationService.GetReservationsAggregate(requestedScreeningId);
        return reservationsAggregate.GetAvailableSeats();
    }
}