using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Application.Handlers;
using Booking.Application.Models;
using Booking.Application.Repositories;
using Booking.Application.Services;
using Booking.Domain.Models;
using NSubstitute;

namespace Booking.UnitTests.Application;

public class CreateMultipleReservationsHandlerTests
{
    private readonly CreateMultipleReservationsHandler _createReservationHandler;
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationService _reservationService;

    public CreateMultipleReservationsHandlerTests()
    {
        _reservationService = Substitute.For<IReservationService>();
        _reservationRepository = Substitute.For<IReservationRepository>();
        _createReservationHandler = new CreateMultipleReservationsHandler(_reservationService, _reservationRepository);
    }

    [Fact]
    public async Task CreateReservations()
    {
        var screeningId1 = 1;
        var screeningId2 = 2;
        var reservationRequest1 = new ReservationRequest(ScreeningId: screeningId1, SeatId: 1);
        var reservationRequest2 = new ReservationRequest(ScreeningId: screeningId1, SeatId: 2);
        var reservationRequest3 = new ReservationRequest(ScreeningId: screeningId2, SeatId: 2);
        var reservationsAggregate1 = new ReservationsAggregate(screeningId1, 
        [
            new Seat(Id: 1, Row: "A", Number: 1, Reservation: null), 
            new Seat(Id: 2, Row: "A", Number: 2, Reservation: null)
        ]);
        var reservationsAggregate2 = new ReservationsAggregate(screeningId2, 
        [
            new Seat(Id: 1, Row: "A", Number: 1, Reservation: null), 
            new Seat(Id: 2, Row: "A", Number: 2, Reservation: null)
        ]);
        var createdReservations = new Reservation.Existing[]
        {
            new (Id: 1, ScreeningId: screeningId1, SeatId: reservationRequest1.SeatId),
            new (Id: 2, ScreeningId: screeningId1, SeatId: reservationRequest2.SeatId),
            new (Id: 3, ScreeningId: screeningId2, SeatId: reservationRequest3.SeatId)
        };
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId1)).Returns(reservationsAggregate1);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId2)).Returns(reservationsAggregate2);
        _reservationRepository.SaveReservations(Arg.Any<IReadOnlyList<Reservation.New>>()).Returns(createdReservations);

        var actual = await _createReservationHandler.Handle(reservationRequests: [reservationRequest1, reservationRequest2, reservationRequest3]);

        _reservationRepository
            .Received(1)
            .SaveReservations(Arg.Is<IReadOnlyList<Reservation.New>>(arg =>
                arg!.SequenceEqual(new Reservation.New[]
                {
                    new (ScreeningId: screeningId1, SeatId: reservationRequest1.SeatId),
                    new (ScreeningId: screeningId1, SeatId: reservationRequest2.SeatId),
                    new (ScreeningId: screeningId2, SeatId: reservationRequest3.SeatId),
                })));
        Assert.Equal(expected: createdReservations, actual);
    }

    [Fact]
    public async Task CreateReservations_WithSeatAlreadyReserved()
    {
        var screeningId1 = 1;
        var reservationRequest1 = new ReservationRequest(ScreeningId: screeningId1, SeatId: 1);
        var reservationRequest2 = new ReservationRequest(ScreeningId: screeningId1, SeatId: 2);
        var seat1 = new Seat(Id: 1, Row: "A", Number: 1, Reservation: new Reservation.Existing(Id: 1, ScreeningId: screeningId1, SeatId: 1));
        var seat2 = new Seat(Id: 2, Row: "A", Number: 2, Reservation: null);
        var createdReservations = new Reservation.Existing[]
        {
            new (Id: 1, ScreeningId: screeningId1, SeatId: reservationRequest1.SeatId),
            new (Id: 2, ScreeningId: screeningId1, SeatId: reservationRequest2.SeatId)
        };
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId1)).Returns(new ReservationsAggregate(screeningId1, [seat1, seat2]));
        _reservationRepository.SaveReservations(Arg.Any<IReadOnlyList<Reservation.New>>()).Returns(createdReservations);

        var actual = () =>  _createReservationHandler.Handle(reservationRequests: [reservationRequest1, reservationRequest2]);

        await Assert.ThrowsAsync<InvalidOperationException>(actual);
    }

    [Fact]
    public async Task CreateReservations_WithNotExistingSeat()
    {
        var screeningId1 = 1;
        var reservationRequest1 = new ReservationRequest(ScreeningId: screeningId1, SeatId: 1);
        var reservationRequest2 = new ReservationRequest(ScreeningId: screeningId1, SeatId: 3);
        var seat1 = new Seat(Id: 1, Row: "A", Number: 1, Reservation: new Reservation.Existing(Id: 1, ScreeningId: screeningId1, SeatId: 1));
        var seat2 = new Seat(Id: 2, Row: "A", Number: 2, Reservation: null);
        var createdReservations = new Reservation.Existing[]
        {
            new (Id: 1, ScreeningId: screeningId1, SeatId: reservationRequest1.SeatId),
            new (Id: 2, ScreeningId: screeningId1, SeatId: reservationRequest2.SeatId)
        };
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId1)).Returns(new ReservationsAggregate(screeningId1, [seat1, seat2]));
        _reservationRepository.SaveReservations(Arg.Any<IReadOnlyList<Reservation.New>>()).Returns(createdReservations);

        var actual = () =>  _createReservationHandler.Handle(reservationRequests: [reservationRequest1, reservationRequest2]);

        await Assert.ThrowsAsync<InvalidOperationException>(actual);
    }
}
