using Booking.Domain.Models;
using Booking.Infrastructure.Persistence.Models;

namespace Booking.Infrastructure.Persistence.Mappers;

public static class ReservationMapper
{
    public static Reservation ToDomain(this ReservationEntity reservationEntity) => new(
        Id: reservationEntity.Id,
        ScreeningId: reservationEntity.ScreeningId,
        SeatId: reservationEntity.SeatId);  
}