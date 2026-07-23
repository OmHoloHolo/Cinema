using System.Collections.Generic;

namespace Show.Api.Responses;

public record SeatsResponse(IReadOnlyList<SeatsResponse.Seat> Seats)
{
    public record Seat(int Id, string Row, int Number);
}