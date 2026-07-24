using System.Collections.Generic;

namespace Booking.Api.Responses;

public record MultipleReservationsResponse(IReadOnlyList<MultipleReservationsResponse.Reservation> Reservations)
{
    public record Reservation(int Id, int ScreeningId, int SeatId);
}