using System.Collections.Generic;

namespace Show.Infrastructure.Persistence.Models;

public record RoomEntity(int Id, int Number)
{
	public List<SeatEntity> Seats { get; set; } = [];
}
