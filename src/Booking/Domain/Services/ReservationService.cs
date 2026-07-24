using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Domain.Services;

public class ReservationService(IReservationRepository reservationRepository, ISeatService seatsService, IRandomProvider randomProvider)
: IReservationService
{
    public bool CancelReservation(int reservationId) =>
        reservationRepository.CancelReservation(reservationId);

    public async Task<Reservation?> CreateReservation(int requestedScreeningId, int? requestedSeatId)
    {
        var availableSeats = await seatsService.GetAvailableSeats(requestedScreeningId);
        if (availableSeats.Count == 0)
            return null;

        var seatId = requestedSeatId ?? GetRandomSeat(availableSeats).Id;
        return !availableSeats.Any(availableSeat => availableSeat.Id == seatId)
            ? null
            : reservationRepository.CreateReservation(requestedScreeningId, seatId);
    }

    public async Task<IReadOnlyList<Reservation>?> CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests)
    {
        foreach (var group in reservationRequests.GroupBy(r => r.ScreeningId))
        {
            var availableSeats = await seatsService.GetAvailableSeats(group.Key);
            if (availableSeats.Count == 0)
                return null;            
               
            if(group.Any(reservationRequest => !availableSeats.Any(availableSeat => availableSeat.Id == reservationRequest.SeatId)))
                return null;
        }

        return reservationRepository.CreateReservations(reservationRequests);
    }

    private Seat GetRandomSeat(IReadOnlyList<Seat> availableSeats)
    {
        var randomIndex = randomProvider.Next(0, availableSeats.Count);
        return availableSeats[randomIndex];
    }
}