using System.Threading.Tasks;
using Booking.Application.Services;
using Booking.Application.Repositories;
using System.Linq;

namespace Booking.Application.Handlers;

public class CancelReservationHandler(
    IReservationRepository reservationRepository, 
    IReservationService reservationService) : ICancelReservationHandler
{
    public async Task Handle(int screeningId, int reservationId)
    {
        var reservationsAggregate = await reservationService.GetReservationsAggregate(screeningId);
        reservationsAggregate.RemoveReservation(reservationId);
        var removedReservations = reservationsAggregate.GetRemovedReservations();
        reservationRepository.DeleteReservation(removedReservations.Single());
    }     
}
