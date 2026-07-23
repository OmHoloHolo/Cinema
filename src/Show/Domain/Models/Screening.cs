using System;

namespace Show.Domain.Models;

public record Screening(int Id, Movie Movie, Room Room, DateTime StartTime);