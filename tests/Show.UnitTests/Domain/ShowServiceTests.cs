using System;
using System.Collections.Generic;
using NSubstitute;
using Show.Domain.Abstractions;
using Show.Domain.Models;
using Show.Domain.Services;

namespace Show.UnitTests.Domain;

public class ShowServiceTests
{
    private readonly IScreeningRepository _screeningRepository;
    private readonly IShowService _showService;

    public ShowServiceTests()
    {
        _screeningRepository = Substitute.For<IScreeningRepository>();
        _showService = new ShowService(_screeningRepository);
    }

    [Fact]
    public void GetScreenings()
    {
        var room = new Room(Id: 1, Number: 1, Seats: []);
        var movie = new Movie(Id: 1, Title: "Beautiful Movie");
        var screenings = new Screening[]
        {
            new(Id: 1, Movie: movie, Room: room, StartTime: new DateTime(2026, 7, 24, 20, 0, 0)),
            new(Id: 2, Movie: movie, Room: room, StartTime: new DateTime(2026, 7, 24, 22, 0, 0))
        };
        _screeningRepository.GetScreenings().Returns(screenings);

        var actual = _showService.GetScreenings();

        Assert.Equal(expected: screenings, actual);
    }

    [Fact]
    public void GetSeats()
    {
        var screeningId = 1;
        var seats = new Seat[]
        {
            new(Id: 1, Row: "A", Number: 1),
            new(Id: 2, Row: "A", Number: 2)
        };
        _screeningRepository.GetSeats(Arg.Is(screeningId)).Returns(seats);

        var actual = _showService.GetSeats(screeningId);

        Assert.Equal(expected: seats, actual);
    }
}
