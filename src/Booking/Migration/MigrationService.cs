using Booking.Infrastructure.Persistence;

namespace Booking.Migration;

public class MigrationService(BookingDbContext dbContext)
{
    public void Migrate()
    {
        dbContext.Database.EnsureCreated();
    }
}