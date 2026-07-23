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

    public int? CreateReservation(int screeningId, int seatId) =>
        reservationRepository.CreateReservation(screeningId, seatId); 

    public async Task<int?> CreateReservation(int screeningId)
    {
        var allSeats = await showProvider.GetSeats(screeningId);
        var reservedSeats = reservationRepository.GetReservedSeats(screeningId);
        var seat = GetRandomAvailableSeat(allSeats, reservedSeats);
        return seat is null 
            ? null
            : reservationRepository.CreateReservation(screeningId, seat.Id);
    }

    public async Task<IReadOnlyList<Seat>> GetAvailableSeats(int screeningId)
    {
        var allSeats = await showProvider.GetSeats(screeningId);
        var reservedSeats = reservationRepository.GetReservedSeats(screeningId);
        return GetAvailableSeats(allSeats, reservedSeats);
    }

    private static IReadOnlyList<Seat> GetAvailableSeats(IReadOnlyList<Seat> allSeats, IReadOnlyList<Seat> reservedSeats) =>
        allSeats.Except(reservedSeats).ToList();

    private static Seat? GetRandomAvailableSeat(IReadOnlyList<Seat> allSeats, IReadOnlyList<Seat> reservedSeats)
    {
        var availableSeats = GetAvailableSeats(allSeats, reservedSeats);
        if (availableSeats.Count == 0)        
            return null;

        var random = new Random();
        var randomIndex = random.Next(availableSeats.Count);
        return availableSeats[randomIndex];
    }
}