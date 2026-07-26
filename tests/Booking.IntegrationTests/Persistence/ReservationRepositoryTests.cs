using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Booking.Domain.Models;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Persistence.Models;
using Booking.Infrastructure.Persistence.Repositories;
using System.Linq;
using System.IO;
using System;

namespace Booking.IntegrationTests.Persistence;

public class ReservationRepositoryTests
{
    [Fact]
    public void CancelReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: 1, SeatId: 1);        
        var reservationEntity = new ReservationEntity(ScreeningId: 1, SeatId: 1) { Id = 1 };        
        dbContext.Reservations.Add(reservationEntity);
        dbContext.SaveChanges();

        reservationRepository.DeleteReservation(existingReservation);

        Assert.Empty(dbContext.Reservations.ToList());
    }

    [Fact]
    public void CancelReservation_NotExistingReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var existingReservation = new Reservation.Existing(Id: 1, ScreeningId: 1, SeatId: 1);          
        var reservationEntity = new ReservationEntity(ScreeningId: 1, SeatId: 1) { Id = 2 };        
        dbContext.Reservations.Add(reservationEntity);
        dbContext.SaveChanges();

        var actual = () => reservationRepository.DeleteReservation(existingReservation);

        Assert.Throws<InvalidOperationException>(actual);
    }

    [Fact]
    public void CreateReservation_CreateNewReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var newReservation = new Reservation.New(ScreeningId: 1, SeatId: 1);

        var actual = reservationRepository.SaveReservations([newReservation]);

        Assert.Equal(expected: 1, actual[0].ScreeningId);
        Assert.Equal(expected: 1, actual[0].SeatId);
    }

    [Fact]
    public void CreateReservation_CreateAlreadyExistingReservation()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var screeningId = 1;
        var seatId = 1;
        var newReservation = new Reservation.New(ScreeningId: screeningId, SeatId: seatId);
        var reservationEntity = new ReservationEntity(ScreeningId: screeningId, SeatId: seatId) { Id = 1 };        
        dbContext.Reservations.Add(reservationEntity);
        dbContext.SaveChanges();

        var actual = () => reservationRepository.SaveReservations([newReservation]);

        Assert.Throws<InvalidOperationException>(actual);
    }

    [Fact]
    public void CreateReservations_CreateNewReservations()
    {
        var dbContext = CreateDbContext();        
        var reservationRepository = new ReservationRepository(dbContext);

        var screeningId = 1;
        var seat1Id = 1;
        var seat2Id = 2;
        var reservationRequests = new Reservation.New[]
        {
            new (ScreeningId: 1, SeatId: 1),
            new (ScreeningId: 1, SeatId: 2),
        };

        var actual = reservationRepository.SaveReservations(reservationRequests);

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
        var reservationRequests = new Reservation.New[]
        {
            new (ScreeningId: 1, SeatId: 1),
            new (ScreeningId: 1, SeatId: 2),
        };

        var actual = () => reservationRepository.SaveReservations(reservationRequests);

        Assert.Throws<InvalidOperationException>(actual);
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

        var expected = new Reservation.Existing[]
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