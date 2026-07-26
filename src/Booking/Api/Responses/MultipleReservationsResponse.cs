using System.Collections.Generic;

namespace Booking.Api.Responses;

public record ReservationsResponse(IReadOnlyList<ReservationsResponse.Reservation> Reservations)
{
    public record Reservation(int Id, int ScreeningId, int SeatId);
}