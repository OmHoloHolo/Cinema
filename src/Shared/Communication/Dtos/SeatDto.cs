using System.Collections.Generic;

namespace Shared.Communication.Dtos;

public record SeatDto(IReadOnlyList<SeatDto.Seat> Seats)
{
    public record Seat(int Id, string Row, int Number);
}