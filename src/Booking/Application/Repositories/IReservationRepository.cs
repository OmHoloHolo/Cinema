using Booking.Application.Models;
using Booking.Domain.Models;
using System.Collections.Generic;

namespace Booking.Application.Repositories;

public interface IReservationRepository
{
    Reservation.Existing? GetReservation(int reservationId);
    IReadOnlyList<Reservation.Existing> GetReservations(int screeningId);
    IReadOnlyList<Reservation.Existing> SaveReservations(IReadOnlyList<Reservation.New> reservations);
    void DeleteReservation(Reservation.Existing existingReservation);
}