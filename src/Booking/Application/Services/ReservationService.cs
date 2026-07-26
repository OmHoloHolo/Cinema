using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Application.Models;
using Booking.Application.Repositories;
using Booking.Application.Gateways;
using Booking.Domain.Models;

namespace Booking.Application.Services;

public class ReservationService(IShowGateway showGateway, IReservationRepository reservationRepository) : IReservationService
{
    public async Task<ReservationsAggregate> GetReservationsAggregate(int screeningId)
    {
        var allSeatSlots = await showGateway.GetSeatSlots(screeningId);
        if (allSeatSlots.Count == 0)
            throw new InvalidOperationException($"No seats found for screening {screeningId}.");
        var reservations = reservationRepository.GetReservations(screeningId);
        return CreateReservationsAggregate(screeningId, allSeatSlots, reservations);
    }

    private static ReservationsAggregate CreateReservationsAggregate(
        int screeningId,
        IReadOnlyList<SeatSlot> allSeatSlots, 
        IReadOnlyList<Reservation.Existing> existingReservations)
    {
        var reservationsBySeatId = existingReservations.ToDictionary(er => er.SeatId);
        var seats = allSeatSlots.Select(seatSlot =>
        {
            reservationsBySeatId.TryGetValue(seatSlot.SeatId, out var existingReservation);
            return new Seat(
                        Id: seatSlot.SeatId,
                        Row: seatSlot.Row,
                        Number: seatSlot.Number,
                        Reservation: existingReservation);
        }).ToList();
        return new ReservationsAggregate(screeningId, seats);
    }
}