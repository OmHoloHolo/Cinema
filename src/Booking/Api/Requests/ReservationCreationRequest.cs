namespace Booking.Api.Requests;

public record ReservationCreationRequest(int ScreeningId, int? SeatId);