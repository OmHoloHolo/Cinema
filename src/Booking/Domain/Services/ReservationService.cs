using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Domain.Services;

public class ReservationService(IReservationRepository reservationRepository, ISeatService seatsProvider) 
: IReservationService
{
    public bool CancelReservation(int reservationId) =>
        reservationRepository.CancelReservation(reservationId);

    public async Task<Reservation?> CreateReservation(int requestedScreeningId, int? requestedSeatId)
    {
        var seatId = requestedSeatId ?? (await seatsProvider.GetRandomAvailableSeat(requestedScreeningId))?.Id;
        return !seatId.HasValue
            ? null
            : reservationRepository.CreateReservation(requestedScreeningId, seatId.Value);
    }

    public async Task<IReadOnlyList<Reservation>?> CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests) => 
        reservationRepository.CreateReservations(reservationRequests);
}