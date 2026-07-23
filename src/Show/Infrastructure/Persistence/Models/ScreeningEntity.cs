using System;

namespace Show.Infrastructure.Persistence.Models;

public record ScreeningEntity(int Id, int MovieId, int RoomId, DateTime StartTime)
{
	public MovieEntity Movie { get; set; } = null!;
	public RoomEntity Room { get; set; } = null!;
}