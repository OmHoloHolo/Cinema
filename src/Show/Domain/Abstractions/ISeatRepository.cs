using Show.Domain.Models;
using System.Collections.Generic;

namespace Show.Domain.Abstractions;

public interface ISeatRepository
{
    IReadOnlyList<Seat> GetSeats(int roomId);
}