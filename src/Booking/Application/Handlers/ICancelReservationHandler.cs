using System.Threading.Tasks;

namespace Booking.Application.Handlers;

public interface ICancelReservationHandler
{
    Task Handle(int screeningId, int reservationId);
}
