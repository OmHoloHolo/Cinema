namespace Booking.Infrastructure.Persistence.Models;

public record ReservationEntity(int ScreeningId, int SeatId)
{
    public int Id { get; set; }
}