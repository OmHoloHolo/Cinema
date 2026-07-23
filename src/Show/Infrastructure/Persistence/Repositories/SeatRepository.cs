using System.Collections.Generic;
using System.Linq;
using Show.Domain.Abstractions;
using Show.Domain.Models;

namespace Show.Infrastructure.Persistence.Repositories;

public class SeatRepository(ShowDbContext showDbContext) : ISeatRepository
{
    public IReadOnlyList<Seat> GetSeats(int roomId) => 
        showDbContext.Seats
            .Where(seat => seat.RoomId == roomId)
            .Select(seat => new Seat(Id: seat.Id, Row: seat.Row, Number: seat.Number))
            .ToList();
}