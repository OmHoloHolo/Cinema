using Show.Domain.Abstractions;
using Show.Domain.Models;
using System.Collections.Generic;

namespace Show.Domain.Services;

public class ShowService(IScreeningRepository screeningRepository, ISeatRepository seatRepository) : IShowService
{
    public IReadOnlyList<Screening> GetScreenings() => 
        screeningRepository.GetScreenings();

    public IReadOnlyList<Seat> GetSeats(int roomId) => 
        seatRepository.GetSeats(roomId);
}