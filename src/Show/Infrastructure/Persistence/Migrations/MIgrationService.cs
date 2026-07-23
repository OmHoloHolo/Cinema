using System;
using System.Collections.Generic;
using System.Linq;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence.Migrations;

public class MigrationService(ShowDbContext dbContext)
{
    private static readonly IEnumerable<MovieEntity> DefaultMovies = [ new(Id: 1, Title: "Nice Movie"), new(Id: 2, Title: "Bad Movie"), new(Id: 3, Title: "Best Movie") ]; 
    private static readonly IEnumerable<RoomEntity> DefaultRooms = [ new(Id: 1, Number: 1), new(Id: 2, Number: 2), new(Id: 3, Number: 3) ]; 
    private static readonly IEnumerable<SeatEntity> DefaultSeats = [ 
        new(Id: 1, RoomId: 1, Row: "A", Number: 1), new(Id: 2, RoomId: 1, Row: "A", Number: 2), new(Id: 3, RoomId: 1, Row: "B", Number: 1),
        new(Id: 4, RoomId: 2, Row: "A", Number: 1), new(Id: 5, RoomId: 2, Row: "B", Number: 1), new(Id: 6, RoomId: 2, Row: "B", Number: 2),
        new(Id: 7, RoomId: 3, Row: "A", Number: 1), new(Id: 8, RoomId: 3, Row: "A", Number: 2), new(Id: 9, RoomId: 3, Row: "A", Number: 3),
    ]; 
    private static readonly IEnumerable<ScreeningEntity> DefaultScreenings = [ 
        new(Id: 1, MovieId: 1, RoomId: 1, StartTime: new DateTime(2026, 7, 23, 18, 30, 0)), 
        new(Id: 2, MovieId: 1, RoomId: 2, StartTime: new DateTime(2026, 7, 23, 20, 0, 0)), 
        new(Id: 3, MovieId: 2, RoomId: 1, StartTime: new DateTime(2026, 7, 23, 21, 30, 0)), 
        new(Id: 4, MovieId: 3, RoomId: 3, StartTime: new DateTime(2026, 7, 23, 21, 0, 0)), 
    ]; 

    public void Migrate()
    {
        dbContext.Database.EnsureCreated();

        if(!dbContext.Movies.Any())
            dbContext.Movies.AddRange(DefaultMovies);

        if(!dbContext.Rooms.Any())
            dbContext.Rooms.AddRange(DefaultRooms);

        if(!dbContext.Seats.Any())
            dbContext.Seats.AddRange(DefaultSeats);

        if(!dbContext.Screenings.Any())
            dbContext.Screenings.AddRange(DefaultScreenings);

        dbContext.SaveChanges();
    }
}