using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.Repositories;
using Booking.Application.Services;
using Booking.Domain.Models;

namespace Booking.Application.Handlers;

public class CreateReservationHandler(
    IReservationService reservationService,
    IReservationRepository reservationRepository,
    IRandomGenerator randomGenerator)
{
    public async Task<IReadOnlyList<Reservation.Existing>> Handle(int requestedScreeningId, int? requestedSeatId)
    {
        var reservationsAggregate = await reservationService.GetReservationsAggregate(requestedScreeningId);
        if(requestedSeatId.HasValue) 
            reservationsAggregate.ReserveSeat(requestedSeatId.Value);
        else
            reservationsAggregate.ReserveRandomSeat(randomGenerator.Next);

        return reservationRepository.SaveReservations(reservationsAggregate.GetAddedReservations());
    }
}