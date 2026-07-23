using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Domain.Services;

public class BookingService(IReservationRepository reservationRepository, IShowProvider showProvider) : IBookingService
{
    public bool CancelReservation(int reservationId) =>
        reservationRepository.CancelReservation(reservationId);

    public Reservation? CreateReservation(int screeningId, int seatId) =>
        reservationRepository.CreateReservation(screeningId, seatId); 

    public async Task<Reservation?> CreateReservation(int screeningId)
    {
        var allSeats = await showProvider.GetSeats(screeningId);
        var reservedSeatIds = reservationRepository
            .GetReservations(screeningId)
            .Select(r => r.SeatId)
            .ToList();
        var seat = GetRandomAvailableSeat(allSeats, reservedSeatIds);
        return seat is null 
            ? null
            : reservationRepository.CreateReservation(screeningId, seat.Id);
    }

    public async Task<IReadOnlyList<Reservation>?> CreateReservations(IReadOnlyList<ReservationRequest> reservationRequests) => 
        reservationRepository.CreateReservations(reservationRequests);

    public async Task<IReadOnlyList<Seat>> GetAvailableSeats(int screeningId)
    {
        var allSeats = await showProvider.GetSeats(screeningId);
        var reservedSeatIds = reservationRepository
            .GetReservations(screeningId)
            .Select(r => r.SeatId)
            .ToList();
        return GetAvailableSeats(allSeats, reservedSeatIds);
    }

    private static IReadOnlyList<Seat> GetAvailableSeats(IReadOnlyList<Seat> allSeats, IReadOnlyList<int> reservedSeatIds) =>
        allSeats.ExceptBy(reservedSeatIds, seat => seat.Id).ToList();

    private static Seat? GetRandomAvailableSeat(IReadOnlyList<Seat> allSeats, IReadOnlyList<int> reservedSeatIds)
    {
        var availableSeats = GetAvailableSeats(allSeats, reservedSeatIds);
        if (availableSeats.Count == 0)        
            return null;

        var random = new Random();
        var randomIndex = random.Next(availableSeats.Count);
        return availableSeats[randomIndex];
    }
}