using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Booking.Domain.Models;

public class ReservationsAggregate
{
    public int ScreeningId { get; }
    private Dictionary<int, Seat> SeatsById { get; }
    private readonly List<Reservation.Existing> RemovedReservations = [];

    public ReservationsAggregate(int screeningId, IReadOnlyList<Seat> seats)
    {
        ScreeningId = screeningId;
        SeatsById = seats.ToDictionary(seat => seat.Id);
    }

    public IReadOnlyList<Seat> GetAvailableSeats() => SeatsById.Values
        .Where(seat => seat.Reservation is null)
        .ToList();

    public IReadOnlyList<Reservation.New> GetAddedReservations() => SeatsById.Values
        .Select(seat => seat.Reservation)
        .OfType<Reservation.New>()
        .ToList();

    public IReadOnlyList<Reservation.Existing> GetRemovedReservations() => RemovedReservations;

    public void ReserveSeat(int seatId)
    {
        if (!SeatsById.TryGetValue(seatId, out var seat))
            throw new InvalidDataException($"Seat with id {seatId} does not exist in this screening.");
        else
        {
            if (seat.Reservation is not null)
                throw new InvalidOperationException($"Seat with id {seatId} is already reserved.");

            SeatsById[seatId] = seat with { Reservation = new Reservation.New(ScreeningId, seatId) };
        }
    }

    public void ReserveRandomSeat(Func<int, int> getRandomIndex)
    {
        var availableSeats = GetAvailableSeats();
        if (availableSeats.Count == 0)
            throw new InvalidOperationException($"No available seats for screening {ScreeningId}.");

        var seatId = availableSeats[getRandomIndex(availableSeats.Count)].Id;
        ReserveSeat(seatId);
    }

    public void RemoveReservation(int reservationId)
    {
        var reservationToRemove = SeatsById.Values
            .Select(seat => seat.Reservation)
            .OfType<Reservation.Existing>()
            .SingleOrDefault(reservation => reservation.Id == reservationId)
            ?? throw new InvalidDataException($"No existing reservation with {reservationId}.");
        
        RemovedReservations.Add(reservationToRemove);
        SeatsById[reservationToRemove.SeatId] = SeatsById[reservationToRemove.SeatId] with { Reservation = null };
    }
}
