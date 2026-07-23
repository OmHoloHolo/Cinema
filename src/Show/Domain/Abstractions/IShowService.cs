using Show.Domain.Models;
using System.Collections.Generic;

namespace Show.Domain.Abstractions;

public interface IShowService
{
    IReadOnlyList<Screening> GetScreenings();
    IReadOnlyList<Seat> GetSeats(int roomId);
}