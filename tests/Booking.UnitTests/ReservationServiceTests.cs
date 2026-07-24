using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using Booking.Domain.Services;
using NSubstitute;

namespace Booking.UnitTests;

public class ReservationServiceTests
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ISeatService _seatService;
    private readonly IReservationService _reservationService;

    public ReservationServiceTests()
    {
        _reservationRepository = Substitute.For<IReservationRepository>(); 
        _seatService = Substitute.For<ISeatService>(); 
        _reservationService = new ReservationService(_reservationRepository, _seatService);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CancelReservation(bool cancelReservationResult)
    {
        var reservationId = 1;
        _reservationRepository.CancelReservation(Arg.Is(reservationId)).Returns(cancelReservationResult);
        
        var actual = _reservationService.CancelReservation(reservationId);

        Assert.Equal(expected: cancelReservationResult, actual);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    public async Task CreateReservation(int screeningId, int? seatId)
    {
        var availableSeat = new Seat(Id: seatId ?? 2, Row: "A", Number: 1);
        var createdReservation = new Reservation(Id: 1, ScreeningId: screeningId, SeatId: availableSeat.Id);
        _seatService.GetRandomAvailableSeat(Arg.Is(screeningId)).Returns(availableSeat);
        _reservationRepository.CreateReservation(Arg.Is(screeningId), Arg.Is(availableSeat.Id)).Returns(createdReservation);
        
        var actual = await _reservationService.CreateReservation(screeningId, seatId);

        Assert.Equal(expected: createdReservation, actual);
    }

    [Fact]
    public async Task CreateReservation_WithNoSeatRequestedAndNoAvailableSeats()
    {
        var screeningId = 1;
        var seatId = 2;
        _seatService.GetRandomAvailableSeat(Arg.Is(screeningId)).Returns((Seat?)null);
        
        var actual = await _reservationService.CreateReservation(screeningId, seatId);

        Assert.Null(actual);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    public async Task CreateReservation_WithReservationNotCreated(int screeningId, int? seatId)
    {
        _seatService.GetRandomAvailableSeat(Arg.Is(screeningId)).Returns(new Seat(Id: 2, Row: "A", Number: 1));
        _reservationRepository.CreateReservation(Arg.Any<int>(), Arg.Any<int>()).Returns((Reservation?)null);
        
        var actual = await _reservationService.CreateReservation(screeningId, seatId);

        Assert.Null(actual);
    }

    [Fact]
    public async Task CreateReservations()
    {
        var reservationRequests = new ReservationRequest[]
        {          
            new (ScreeningId: 1, SeatId: 2),
            new (ScreeningId: 1, SeatId: 3)
        };
        var createdReservations = new Reservation[]
        {
            new (Id: 1, ScreeningId: 1, SeatId: 2),
            new (Id: 2, ScreeningId: 1, SeatId: 3)
        };
        _reservationRepository.CreateReservations(Arg.Is(reservationRequests)).Returns(createdReservations);
        
        var actual = await _reservationService.CreateReservations(reservationRequests);

        Assert.Equal(expected: createdReservations, actual);
    }

    [Fact]
    public async Task CreateReservations_WithReservationsNotCreated()
    {
        var reservationRequests = new ReservationRequest[]
        {          
            new (ScreeningId: 1, SeatId: 2),
            new (ScreeningId: 1, SeatId: 3)
        };
        _reservationRepository.CreateReservations(Arg.Is(reservationRequests)).Returns((Reservation[]?)null);
        
        var actual = await _reservationService.CreateReservations(reservationRequests);

        Assert.Null(actual);
    }
}
