using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using Booking.Domain.Services;
using NSubstitute;

namespace Booking.UnitTests.Domain;

public class SeatServiceTests
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IShowProvider _showProvider;
    private readonly ISeatService _seatService;
    private readonly IRandomProvider _randomProvider;

    public SeatServiceTests()
    {
        _reservationRepository = Substitute.For<IReservationRepository>(); 
        _showProvider = Substitute.For<IShowProvider>(); 
        _randomProvider = Substitute.For<IRandomProvider>(); 
        _seatService = new SeatService(_showProvider, _reservationRepository, _randomProvider);
    }

    [Fact]
    public async Task GetAvailableSeats()
    {
        var screeningId = 1;
        var seats = new Seat[]
        {
            new(Id: 1, Row: "A", Number: 1),
            new(Id: 2, Row: "A", Number: 2)
        };
        var reservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: screeningId, SeatId: 2)
        };
        _showProvider.GetSeats(Arg.Is(screeningId)).Returns(seats);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns(reservations);
        
        var actual = await _seatService.GetAvailableSeats(screeningId);

        var expected = new Seat[]
        {
            new(Id: 1, Row: "A", Number: 1)
        };
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public async Task GetAvailableSeats_NoSeatsFromProvider()
    {
        var screeningId = 1;
        var reservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: screeningId, SeatId: 2),
            new(Id: 1, ScreeningId: screeningId, SeatId: 3)
        };
        _showProvider.GetSeats(Arg.Is(screeningId)).Returns([]);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns(reservations);
        
        var actual = await _seatService.GetAvailableSeats(screeningId);

        Assert.Equal(expected: [], actual);
    }

    [Fact]
    public async Task GetAvailableSeats_AllSeatsReserved()
    {
        var screeningId = 1;
        var seats = new Seat[]
        {
            new(Id: 1, Row: "A", Number: 1),
            new(Id: 2, Row: "A", Number: 2)
        };
        var reservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: screeningId, SeatId: 1),
            new(Id: 2, ScreeningId: screeningId, SeatId: 2),
        };
        _showProvider.GetSeats(Arg.Is(screeningId)).Returns(seats);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns(reservations);
        
        var actual = await _seatService.GetAvailableSeats(screeningId);

        Assert.Equal(expected: [], actual);
    }

    [Fact]
    public async Task GetRandomAvailableSeats()
    {
        var screeningId = 1;
        var seats = new Seat[]
        {
            new(Id: 1, Row: "A", Number: 1),
            new(Id: 2, Row: "A", Number: 2),
            new(Id: 3, Row: "B", Number: 1)
        };
        var reservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: screeningId, SeatId: 2)
        };
        _showProvider.GetSeats(Arg.Is(screeningId)).Returns(seats);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns(reservations);
        _randomProvider.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(3);
        
        var actual = await _seatService.GetRandomAvailableSeat(screeningId);

        var expected = new Seat(Id: 3, Row: "B", Number: 1);
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public async Task GetRandomAvailableSeat_NoSeatsFromProvider()
    {
        var screeningId = 1;
        var reservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: screeningId, SeatId: 2),
            new(Id: 1, ScreeningId: screeningId, SeatId: 3)
        };
        _showProvider.GetSeats(Arg.Is(screeningId)).Returns([]);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns(reservations);
        
        var actual = await _seatService.GetRandomAvailableSeat(screeningId);

        Assert.Null(actual);
    }

    [Fact]
    public async Task GetRandomAvailableSeat_AllSeatsReserved()
    {
        var screeningId = 1;
        var seats = new Seat[]
        {
            new(Id: 1, Row: "A", Number: 1),
            new(Id: 2, Row: "A", Number: 2)
        };
        var reservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: screeningId, SeatId: 1),
            new(Id: 2, ScreeningId: screeningId, SeatId: 2),
        };
        _showProvider.GetSeats(Arg.Is(screeningId)).Returns(seats);
        _reservationRepository.GetReservations(Arg.Is(screeningId)).Returns(reservations);
        
        var actual = await _seatService.GetRandomAvailableSeat(screeningId);

        Assert.Null(actual);
    }
}
