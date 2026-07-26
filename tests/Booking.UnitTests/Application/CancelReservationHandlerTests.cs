using System.IO;
using System.Threading.Tasks;
using Booking.Application.Handlers;
using Booking.Application.Repositories;
using Booking.Application.Services;
using Booking.Domain.Models;
using NSubstitute;

namespace Booking.UnitTests.Application;

public class CancelReservationHandlerTests
{
    private readonly CancelReservationHandler _cancelReservationHandler;
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationService _reservationService;

    public CancelReservationHandlerTests()
    {
        _reservationService = Substitute.For<IReservationService>(); 
        _reservationRepository = Substitute.For<IReservationRepository>(); 
        _cancelReservationHandler = new CancelReservationHandler(_reservationRepository, _reservationService);
    }

    [Fact]
    public async Task CancelReservation()
    {
        var screeningId = 1;
        var existingReservation = new Reservation.Existing(
            Id: 1, 
            ScreeningId: screeningId, 
            SeatId: 2);
        var seat = new Seat(Id: 2, Row: "A", Number: 1, Reservation: existingReservation);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat]);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        
        await _cancelReservationHandler.Handle(screeningId, existingReservation.Id);

        _reservationRepository
            .Received(1)
            .DeleteReservation(Arg.Is(existingReservation));
    }

    [Fact]
    public async Task CancelReservation_WithNotExistingReservation()
    {
        var screeningId = 1;
        var reservationId = 1;
        var seat = new Seat(Id: 2, Row: "A", Number: 1, Reservation: null);
        var reservationsAggregate = new ReservationsAggregate(screeningId, [seat]);
        _reservationService.GetReservationsAggregate(Arg.Is(screeningId)).Returns(reservationsAggregate);
        
        var actual = async () => await _cancelReservationHandler.Handle(screeningId, reservationId);

        await Assert.ThrowsAsync<InvalidDataException>(actual);
    }
}
