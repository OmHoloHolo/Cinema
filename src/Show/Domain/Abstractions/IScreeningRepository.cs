using Show.Domain.Models;
using System.Collections.Generic;

namespace Show.Domain.Abstractions;

public interface IScreeningRepository
{
    IReadOnlyList<Screening> GetScreenings();
    IReadOnlyList<Seat> GetSeats(int screeningId);
}
