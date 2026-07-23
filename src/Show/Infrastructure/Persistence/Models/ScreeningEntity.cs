using System;
using Show.Domain.Models;

namespace Show.Infrastructure.Persistence.Models;

public record ScreeningEntity(int Id, MovieEntity Movie, RoomEntity Room, DateTime StartTime);