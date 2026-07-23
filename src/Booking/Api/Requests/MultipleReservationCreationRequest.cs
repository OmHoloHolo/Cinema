using System.Collections.Generic;

namespace Booking.Api.Requests;

public record MultipleReservationCreationRequest(IReadOnlyList<MultipleReservationCreationRequest.Reservation> Reservations)
{
    public record Reservation(int ScreeningId, int SeatId);
}