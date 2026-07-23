using System.Collections.Generic;

namespace Booking.Api.Responses;

public record AvailableSeatsResponse(IReadOnlyList<AvailableSeatsResponse.Seat> Seats)
{
    public record Seat(int Id, string Row, int Number);
}
