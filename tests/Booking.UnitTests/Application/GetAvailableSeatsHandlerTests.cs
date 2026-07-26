using System;
using System.IO;
using System.Threading.Tasks;
using Booking.Application.Handlers;
using Booking.Application.Services;
using Booking.Domain.Models;
using NSubstitute;

namespace Booking.UnitTests.Application;

public class GetAvailableSeatsHandlerTests
{
    private readonly GetAvailableSeatsHandler _getAvailableSeatsHandler;
    private readonly IReservationService _reservationService;

    public GetAvailableSeatsHandlerTests()
    {
        _reservationService = Substitute.For<IReservationService>(); 
        _getAvailableSeatsHandler = new GetAvailableSeatsHandler(_reservationService);
    }

    [Fact]
    public async Task GetAvailableSeats()
    {
        var screeningId = 1;
        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: screeningId, SeatId: 2);
        var seat1 = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        var seat2 = new Seat(Id: 2, Row: "A", Number: 2, Reservation: existingReservation);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat1, seat2]);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        
        var actual = await _getAvailableSeatsHandler.Handle(screeningId);

        Assert.Equal(expected: [seat1], actual);
    }
}