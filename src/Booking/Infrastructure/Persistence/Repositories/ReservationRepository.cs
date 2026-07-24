using System.Collections.Generic;
using System.Linq;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using Booking.Infrastructure.Persistence.Mappers;
using Booking.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence.Repositories;

public class ReservationRepository(BookingDbContext dbContext) : IReservationRepository
{
    public bool CancelReservation(int reservationId)
    {
        var deletedRow = dbContext.Reservations
            .Where(reservation => reservation.Id == reservationId)
            .ExecuteDelete();
        return deletedRow > 0;
    }

    public Reservation? CreateReservation(int screeningId, int seatId)
    {
        var reservationEntity = new ReservationEntity(ScreeningId: screeningId, SeatId: seatId);
        dbContext.Reservations.Add(reservationEntity);
        try
        {
            dbContext.SaveChanges();
            return reservationEntity.ToDomain();
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public IReadOnlyList<Reservation>? CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests)
    {
        var reservationEntities = reservationRequests
            .Select(reservationRequest => new ReservationEntity(ScreeningId: reservationRequest.ScreeningId, SeatId: reservationRequest.SeatId))
            .ToList();
        dbContext.Reservations.AddRange(reservationEntities);
        try
        {
            dbContext.SaveChanges();
            return reservationEntities
                .Select(reservationEntity => reservationEntity.ToDomain())
                .ToList();
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public IReadOnlyList<Reservation> GetReservations(int screeningId) =>    
        dbContext.Reservations
            .Where(reservation => reservation.ScreeningId == screeningId)
            .Select(reservation => new Reservation(
                Id: reservation.Id,
                ScreeningId: reservation.ScreeningId,
                SeatId: reservation.SeatId))
            .ToList();
}