using Booking.Api.Mappers;
using Booking.Api.Requests;
using Booking.Api.Responses;
using Booking.Domain.Models;

namespace Booking.UnitTests.Api;

public class ApiMapperTests
{
    [Fact]
    public void ToDomain_ReservationRequests()
    {
        var screeningId = 1;
        var seatId = 2;
        var request = new MultipleReservationCreationRequest(Reservations: [new(ScreeningId: screeningId, SeatId: seatId)]);

        var result = request.ToDomain();

        var expected = new ReservationRequest[] { new(ScreeningId: screeningId, SeatId: seatId) };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToResponse_Reservation()
    {
        var reservationId = 1;
        var screeningId = 2;
        var seatId = 3;
        var reservation = new Reservation(Id: reservationId, ScreeningId: screeningId, SeatId: seatId);

        var response = reservation.ToResponse();

        var expected = new ReservationResponse(Id: reservationId, ScreeningId: screeningId, SeatId: seatId);
        Assert.Equivalent(expected, response);
    }

    [Fact]
    public void ToResponse_MultipleReservations()
    {
        var reservationId = 1;
        var screeningId = 2;
        var seatId = 3;
        var reservations = new Reservation[] { new(Id: reservationId, ScreeningId: screeningId, SeatId: seatId) };

        var response = reservations.ToResponse();

        var expected = new MultipleReservationsResponse(Reservations: 
        [
            new(Id: reservationId, ScreeningId: screeningId, SeatId: seatId)
        ]);
        Assert.Equivalent(expected, response);
    }

    [Fact]
    public void ToResponse_Seats()
    {
        var id = 1;
        var row = "A";
        var number = 1;
        var seats = new Seat[] { new(Id: id, Row: row, Number: number) };

        var response = seats.ToResponse();

        var expected = new AvailableSeatsResponse(Seats: [new(Id: id, Row: row, Number: number)]);
        Assert.Equivalent(expected, response);
    }
}
