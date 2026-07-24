using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Booking.Domain.Models;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Persistence.Models;
using Booking.Infrastructure.Persistence.Repositories;

namespace Booking.IntegrationTests.Persistence;

public class ReservationRepositoryTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void CancelReservation(int reservationId, bool expected)
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var reservationEntity = new ReservationEntity(ScreeningId: 1, SeatId: 1) { Id = 1 };        
        dbContext.Reservations.Add(reservationEntity);
        dbContext.SaveChanges();

        var actual = reservationRepository.CancelReservation(reservationId: reservationId);

        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public void CreateReservation_CreateNewReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var screeningId = 1;
        var seatId = 1;

        var actual = reservationRepository.CreateReservation(screeningId: 1, seatId: 1);

        Assert.Equal(expected: screeningId, actual!.ScreeningId);
        Assert.Equal(expected: seatId, actual!.SeatId);
    }

    [Fact]
    public void CreateReservation_CreateAlreadyExistingReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var screeningId = 1;
        var seatId = 1;
        var reservationEntity = new ReservationEntity(ScreeningId: screeningId, SeatId: seatId) { Id = 1 };        
        dbContext.Reservations.Add(reservationEntity);
        dbContext.SaveChanges();

        var actual = reservationRepository.CreateReservation(screeningId: screeningId, seatId: seatId);

        Assert.Null(actual);
    }

    [Fact]
    public void CreateReservations_CreateNewReservations()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var screeningId = 1;
        var seat1Id = 1;
        var seat2Id = 1;
        var reservationRequests = new ReservationRequest[]
        {
            new (ScreeningId: 1, SeatId: 1),
            new (ScreeningId: 1, SeatId: 2),
        };

        var actual = reservationRepository.CreateReservations(reservationRequests);

        Assert.Equal(expected: screeningId, actual![0].ScreeningId);
        Assert.Equal(expected: seat1Id, actual[0].SeatId);
        Assert.Equal(expected: screeningId, actual[1].ScreeningId);
        Assert.Equal(expected: seat2Id, actual[1].SeatId);
    }

    [Fact]
    public void CreateReservations_CreateAlreadyExistingReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var screeningId = 1;
        var seatId = 1;
        var reservationEntity = new ReservationEntity(ScreeningId: screeningId, SeatId: seatId) { Id = 1 };        
        dbContext.Reservations.Add(reservationEntity);
        dbContext.SaveChanges();
        var reservationRequests = new ReservationRequest[]
        {
            new (ScreeningId: 1, SeatId: 1),
            new (ScreeningId: 1, SeatId: 2),
        };

        var actual = reservationRepository.CreateReservations(reservationRequests);

        Assert.Null(actual);
    }

    [Fact]
    public void GetReservations()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var reservation1Entity = new ReservationEntity(ScreeningId: 1, SeatId: 1) { Id = 1 };        
        var reservation2Entity = new ReservationEntity(ScreeningId: 2, SeatId: 2) { Id = 2 };        
        dbContext.Reservations.AddRange([reservation1Entity, reservation2Entity]);
        dbContext.SaveChanges();

        var actual = reservationRepository.GetReservations(screeningId: 1);

        var expected = new Reservation[]
        {
            new(Id: reservation1Entity.Id, ScreeningId: reservation1Entity.ScreeningId, SeatId: reservation1Entity.SeatId)
        };
        Assert.Equal(expected: expected, actual);
    }

    private static BookingDbContext CreateDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var dbContext = new BookingDbContext(new DbContextOptionsBuilder<BookingDbContext>()
            .UseSqlite(connection)
            .Options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}