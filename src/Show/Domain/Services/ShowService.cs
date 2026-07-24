using Show.Domain.Abstractions;
using Show.Domain.Models;
using System.Collections.Generic;

namespace Show.Domain.Services;

public class ShowService(IScreeningRepository screeningRepository) : IShowService
{
    public IReadOnlyList<Screening> GetScreenings() => 
        screeningRepository.GetScreenings();

    public IReadOnlyList<Seat> GetSeats(int screeningId) => 
        screeningRepository.GetSeats(screeningId);
}