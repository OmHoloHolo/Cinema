using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Show.Domain.Abstractions;
using Show.Domain.Models;
using Show.Infrastructure.Persistence;
using Show.Infrastructure.Persistence.Models;
using Show.Infrastructure.Persistence.Repositories;

namespace Show.IntegrationTests.Persistence;

public class ScreeningRepositoryTests
{
    [Fact]
    public void GetScreenings()
    {
        var dbContext = CreateDbContext();
        var screeningRepository = new ScreeningRepository(dbContext);
    
        var movieEntity = new MovieEntity(Id: 1, Title: "Worst Movie");
        var roomEntity = new RoomEntity(Id: 1, Number: 1);
        var seatEntity = new SeatEntity(Id: 1, RoomId: 1, Row: "A", Number: 1);
        var screeningEntity = new ScreeningEntity(Id: 1, MovieId: 1, RoomId: 1, StartTime: new DateTime(2026, 7, 24, 20, 0, 0));
        
        dbContext.Movies.Add(movieEntity);
        dbContext.Rooms.Add(roomEntity);
        dbContext.Seats.Add(seatEntity);
        dbContext.Screenings.Add(screeningEntity);
        dbContext.SaveChanges();

        var actual = screeningRepository.GetScreenings();

        var expected = new Screening[]
        {
            new(
                Id: screeningEntity.Id, 
                Movie: new Movie(
                    Id: movieEntity.Id, 
                    Title: movieEntity.Title), 
                    Room: new Room(
                        Id: roomEntity.Id, 
                        Number: roomEntity.Number, 
                        Seats: [new Seat(
                            Id: seatEntity.Id, 
                            Row: seatEntity.Row, 
                            Number: seatEntity.Number)]), 
                StartTime: screeningEntity.StartTime)
        };
        Assert.Equivalent(expected: expected, actual);
    }

    [Fact]
    public void GetSeats()
    {
        var dbContext = CreateDbContext();
        var screeningRepository = new ScreeningRepository(dbContext);

        var screeningId = 1;
        var movieEntity = new MovieEntity(Id: 1, Title: "Worst Movie");
        var room1Entity = new RoomEntity(Id: 1, Number: 1);
        var room2Entity = new RoomEntity(Id: 2, Number: 2);
        var room1SeatEntity = new SeatEntity(Id: 1, RoomId: 1, Row: "A", Number: 1);
        var room2SeatEntity = new SeatEntity(Id: 2, RoomId: 2, Row: "A", Number: 1);
        var screeningEntity = new ScreeningEntity(Id: screeningId, MovieId: 1, RoomId: 1, StartTime: new DateTime(2026, 7, 24, 20, 0, 0));
        
        dbContext.Movies.Add(movieEntity);
        dbContext.Rooms.AddRange([room1Entity, room2Entity]);
        dbContext.Seats.AddRange([room1SeatEntity, room2SeatEntity]);
        dbContext.Screenings.Add(screeningEntity);
        dbContext.SaveChanges();

        var actual = screeningRepository.GetSeats(screeningId);

        var expected = new Seat[]
        {
            new (
                Id: room1SeatEntity.Id, 
                Row: room1SeatEntity.Row, 
                Number: room1SeatEntity.Number)
        };
        Assert.Equivalent(expected: expected, actual);
    }

    private static ShowDbContext CreateDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var dbContext = new ShowDbContext(new DbContextOptionsBuilder<ShowDbContext>()
            .UseSqlite(connection)
            .Options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}