using System;
using System.Collections.Generic;

namespace Show.Api.Responses;

public record ScreeningResponse(IReadOnlyList<ScreeningResponse.Screening> Screenings)
{
    public record Screening(int Id, string MovieTitle, int RoomNumber, DateTime StartTime);
}