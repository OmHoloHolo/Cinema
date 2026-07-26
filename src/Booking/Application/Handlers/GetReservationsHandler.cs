using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.Repositories;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public class GetReservationsHandler(IReservationRepository reservationRepository) : IGetReservationsHandler
{
    public async Task<IReadOnlyList<Reservation.Existing>> Handle(int requestedScreeningId) => 
        reservationRepository.GetReservations(requestedScreeningId);
}