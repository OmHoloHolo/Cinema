using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Application.Models;
using Booking.Application.Repositories;
using Booking.Application.Services;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public class CreateReservationsHandler(IReservationService reservationService, IReservationRepository reservationRepository)
{
    public async Task<IReadOnlyList<Reservation.Existing>> Handle(IReadOnlyList<ReservationRequest> reservationRequests)
    {
        var addedReservations = new List<Reservation.New>();
        foreach (var group in reservationRequests.GroupBy(r => r.ScreeningId))
        {
            var reservationsAggregate = await reservationService.GetReservationsAggregate(group.Key);
            foreach (var reservationRequest in group)
                reservationsAggregate.ReserveSeat(reservationRequest.SeatId);

            addedReservations.AddRange(reservationsAggregate.GetAddedReservations());                  
        }

        return reservationRepository.SaveReservations(addedReservations);
    }
}