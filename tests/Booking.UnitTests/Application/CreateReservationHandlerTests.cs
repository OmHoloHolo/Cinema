using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Booking.Application.Handlers;
using Booking.Application.Repositories;
using Booking.Application.Services;
using Booking.Domain.Models;
using NSubstitute;

namespace Booking.UnitTests.Application;

public class CreateReservationHandlerTests
{
    private readonly CreateReservationHandler _createReservationHandler;
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationService _reservationService;
    private readonly IRandomGenerator _randomGenerator;

    public CreateReservationHandlerTests()
    {
        _reservationService = Substitute.For<IReservationService>(); 
        _reservationRepository = Substitute.For<IReservationRepository>(); 
        _randomGenerator = Substitute.For<IRandomGenerator>(); 
        _createReservationHandler = new CreateReservationHandler(_reservationService, _reservationRepository, _randomGenerator);
    }

    [Fact]
    public async Task CreateReservation_WithSpecificSeatId()
    {
        var screeningId = 1;
        var seatId = 1;
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var createdReservation = new Reservation.Existing(
            Id: 1, 
            ScreeningId: 1, 
            SeatId: seat.Id);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat]);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        _reservationRepository.SaveReservations(Arg.Any<IReadOnlyList<Reservation.New>>()).Returns([createdReservation]);
        
        var actual = await _createReservationHandler.Handle(screeningId, seatId);

        _reservationRepository
            .Received(1)
            .SaveReservations(Arg.Is<IReadOnlyList<Reservation.New>>(x => 
                x![0] == new Reservation.New(ScreeningId: screeningId, SeatId: seat.Id)));
        Assert.Equal(expected: [createdReservation], actual);
    }

    [Fact]
    public async Task CreateReservation_WithNoSpecificSeatId()
    {
        var screeningId = 1;
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var createdReservation = new Reservation.Existing(
            Id: 1, 
            ScreeningId: 1, 
            SeatId: seat.Id);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat]);
        _randomGenerator.Next(Arg.Is(1)).Returns(0);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        _reservationRepository.SaveReservations(Arg.Any<IReadOnlyList<Reservation.New>>()).Returns([createdReservation]);
        
        var actual = await _createReservationHandler.Handle(screeningId, null);

        _reservationRepository
            .Received(1)
            .SaveReservations(Arg.Is<IReadOnlyList<Reservation.New>>(x => 
                x![0] == new Reservation.New(ScreeningId: screeningId, SeatId: seat.Id)));
        Assert.Equal(expected: [createdReservation], actual);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, null)]
    public async Task CreateReservation_WithNoAvailableSeats(int screeningId, int? seatId)
    {
        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: screeningId, SeatId: 1);
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: existingReservation);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat]);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        
        var actual = async () => await _createReservationHandler.Handle(screeningId, seatId);

        await Assert.ThrowsAsync<InvalidOperationException>(actual);
    }

    [Fact]
    public async Task CreateReservation_WithNotExistingSeat()
    {
        var screeningId = 1;
        var seatId = 1;
        var seat = new Seat(Id: 2, Row: "A", Number: 1, Reservation: null);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat]);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        
        var actual = async () => await _createReservationHandler.Handle(screeningId, seatId);

        await Assert.ThrowsAsync<InvalidDataException>(actual);
    }
}
