using System.Collections.Generic;
using System.Linq;
using Show.Domain.Abstractions;
using Show.Domain.Models;

namespace Show.Infrastructure.Persistence.Repositories;

public class ScreeningRepository(ShowDbContext showDbContext) : IScreeningRepository
{
    public IReadOnlyList<Screening> GetScreenings() => 
        showDbContext.Screenings
            .Select(screening => new Screening(
                Id: screening.Id,
                Movie: new Movie(Id: screening.Movie.Id, Title: screening.Movie.Title),
                Room: new Room(
                    Id: screening.Room.Id,
                    Number: screening.Room.Number,
                    Seats: screening.Room.Seats
                        .Select(seat => new Seat(Id: seat.Id, Row: seat.Row, Number: seat.Number))
                        .ToList()),
                StartTime: screening.StartTime))
            .ToList();
}