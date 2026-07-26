namespace Booking.Domain.Models;

public record Reservation
{
    public record New(int ScreeningId, int SeatId) : Reservation;
    public record Existing(int Id, int ScreeningId, int SeatId) : Reservation;
}