using Booking.Infrastructure.Persistence.Mappers;
using Booking.Infrastructure.Persistence.Models;
using Booking.Domain.Models;

namespace Booking.UnitTests.Persistence;

public class ReservationMapperTests
{
    [Fact]
    public void ToDomain()
    {
        var id = 1;
        var screeningId = 2;
        var seatId = 3;
        var entity = new ReservationEntity(ScreeningId: screeningId, SeatId: seatId) { Id = id };

        var result = entity.ToExistingReservation();

        var expected = new Reservation.Existing(Id: id, ScreeningId: screeningId, SeatId: seatId);
        Assert.Equal(expected, result);
    }
}
