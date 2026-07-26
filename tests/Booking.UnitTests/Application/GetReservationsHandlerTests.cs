using System.Threading.Tasks;
using Booking.Application.Handlers;
using Booking.Application.Repositories;
using Booking.Domain.Models;
using NSubstitute;

namespace Booking.UnitTests.Application;

public class GetReservationsHandlerTests
{
    private readonly GetReservationsHandler _getReservationsHandlerr;
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsHandlerTests()
    {
        _reservationRepository = Substitute.For<IReservationRepository>(); 
        _getReservationsHandlerr = new GetReservationsHandler(_reservationRepository);
    }

    [Fact]
    public async Task GetAvailableSeats()
    {
        var screeningId = 1;
        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: screeningId, SeatId: 2);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns([existingReservation]);
        
        var actual = await _getReservationsHandlerr.Handle(screeningId);

        Assert.Equal(expected: [existingReservation], actual);
    }
}