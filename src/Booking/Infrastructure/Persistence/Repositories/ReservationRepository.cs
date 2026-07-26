using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Application.Repositories;
using Booking.Domain.Models;
using Booking.Infrastructure.Persistence.Mappers;
using Booking.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence.Repositories;

public class ReservationRepository(BookingDbContext dbContext) : IReservationRepository
{
    public void DeleteReservation(Reservation.Existing existingReservation)
    {
        var deletedRows = dbContext.Reservations
            .Where(r => r.Id == existingReservation.Id)
            .ExecuteDelete();
        if(deletedRows == 0)
            throw new InvalidOperationException(
                $"Reservation with Id {existingReservation.Id} not found.");
    }
    
    public Reservation.Existing? GetReservation(int reservationId) =>    
        dbContext.Reservations
            .SingleOrDefault(reservation => reservation.Id == reservationId)?
            .ToExistingReservation();

    public IReadOnlyList<Reservation.Existing> GetReservations(int screeningId) =>    
        dbContext.Reservations
            .Where(reservation => reservation.ScreeningId == screeningId)
            .Select(reservation => reservation.ToExistingReservation())
            .ToList();

    public IReadOnlyList<Reservation.Existing> SaveReservations(IReadOnlyList<Reservation.New> reservations)
    {
        var reservationEntities = reservations
            .Select(reservation => new ReservationEntity(
                ScreeningId: reservation.ScreeningId, 
                SeatId: reservation.SeatId))
            .ToList();
        dbContext.Reservations.AddRange(reservationEntities);
        try
        {
            dbContext.SaveChanges();
            return reservationEntities
                .Select(reservationEntity => reservationEntity.ToExistingReservation())
                .ToList();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Failed to save reservations");
        }
    }
}