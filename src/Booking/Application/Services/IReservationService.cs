using System.Threading.Tasks;
using Booking.Domain.Models;

namespace Booking.Application.Services;

public interface IReservationService
{
    Task<ReservationsAggregate> GetReservationsAggregate(int requestedScreeningId);
}
