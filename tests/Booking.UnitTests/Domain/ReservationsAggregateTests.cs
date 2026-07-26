using System;
using System.IO;
using Booking.Domain.Models;

namespace Booking.UnitTests.Domain;

public class ReservationsAggregateTests
{
    [Fact]
    public void ReserveSeat_AddsNewReservation()
    {
        var screeningId = 1;
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var aggregate = new ReservationsAggregate(screeningId, [seat]);

        aggregate.ReserveSeat(seat.Id);

        var added = aggregate.GetAddedReservations();
        Assert.Equal(new Reservation.New(ScreeningId: screeningId, SeatId: seat.Id), added[0]);
    }

    [Fact]
    public void ReserveSeat_RemovesSeatFromAvailable()
    {
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var aggregate = new ReservationsAggregate(screeningId: 1, [seat]);

        aggregate.ReserveSeat(seat.Id);

        Assert.Empty(aggregate.GetAvailableSeats());
    }

    [Fact]
    public void ReserveSeat_WhenSeatDoesNotExist()
    {
        var aggregate = new ReservationsAggregate(screeningId: 1, seats: []);

        Assert.Throws<InvalidDataException>(() => aggregate.ReserveSeat(seatId: 99));
    }

    [Fact]
    public void ReserveSeat_WhenSeatAlreadyReserved()
    {
        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: 1, SeatId: 1);
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: existingReservation);
        var aggregate = new ReservationsAggregate(screeningId: 1, [seat]);

        Assert.Throws<InvalidOperationException>(() => aggregate.ReserveSeat(seatId: seat.Id));
    }

    [Fact]
    public void ReserveRandomSeat()
    {
        var screeningId = 1;
        var seat1 = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var seat2 = new Seat(Id: 2, Row: "A", Number: 2, Reservation: null);
        var aggregate = new ReservationsAggregate(screeningId, [seat1, seat2]);

        aggregate.ReserveRandomSeat(_ => 1);

        var added = aggregate.GetAddedReservations();  
        Assert.Equal(new Reservation.New(ScreeningId: screeningId, SeatId: seat2.Id), added[0]);
    }

    [Fact]
    public void ReserveRandomSeat_WhenNoSeatsAvailable()
    {
        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: 1, SeatId: 1);
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: existingReservation);
        var aggregate = new ReservationsAggregate(screeningId: 1, [seat]);

        Assert.Throws<InvalidOperationException>(() => aggregate.ReserveRandomSeat(_ => 0));
    }

    [Fact]
    public void RemoveReservation()
    {
        var existingReservation = new Reservation.Existing(Id: 5, ScreeningId: 1, SeatId: 1);
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: existingReservation);
        var aggregate = new ReservationsAggregate(screeningId: 1, [seat]);

        aggregate.RemoveReservation(reservationId: 5);

        var removed = aggregate.GetRemovedReservations();
        Assert.Equal(existingReservation, removed[0]);
    }

    [Fact]
    public void RemoveReservation_MakesSeatAvailableAgain()
    {
        var existingReservation = new Reservation.Existing(Id: 5, ScreeningId: 1, SeatId: 1);
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: existingReservation);
        var aggregate = new ReservationsAggregate(screeningId: 1, [seat]);

        aggregate.RemoveReservation(reservationId: 5);

        var available = aggregate.GetAvailableSeats();
        Assert.Single(available);
        Assert.Equal(seat.Id, available[0].Id);
    }

    [Fact]
    public void RemoveReservation_WhenReservationDoesNotExist()
    {
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var aggregate = new ReservationsAggregate(screeningId: 1, [seat]);

        Assert.Throws<InvalidDataException>(() => aggregate.RemoveReservation(reservationId: 99));
    }
}
