using System;
using System.Collections.Generic;
using Shared.Communication.Dtos;
using Show.Api.Mappers;
using Show.Api.Responses;
using Show.Domain.Models;

namespace Show.UnitTests.Api;

public class ResponseMapperTests
{
    [Fact]
    public void ToResponse_Screenings()
    {
        var screeningId = 1;
        var movie = new Movie(Id: 1, Title: "Wow Movie");
        var room = new Room(Id: 1, Number: 5, Seats: []);
        var startTime = new DateTime(2026, 7, 24, 20, 0, 0);
        var screenings = new Screening[]
        {
            new(Id: screeningId, Movie: movie, Room: room, StartTime: startTime)
        };

        var response = screenings.ToResponse();

        var expected = new ScreeningResponse(Screenings: [new(Id: screeningId, MovieTitle: movie.Title, RoomNumber: room.Number, StartTime: startTime)]);
        Assert.Equivalent(expected, response);
    }

    [Fact]
    public void ToResponse_Seats()
    {
        var id = 1;
        var row = "A";
        var number = 1;
        var seats = new Seat[]
        {
            new(Id: id, Row: row, Number: number)
        };

        var response = seats.ToResponse();

        var expected = new SeatDto(Seats: [new(Id: id, Row: row, Number: number)]);
        Assert.Equivalent(expected, response);
    }
}
