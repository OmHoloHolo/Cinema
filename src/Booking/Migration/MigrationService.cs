using Booking.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Booking.Migration;

public class MigrationService(ILogger<MigrationService> logger, BookingDbContext dbContext)
{
    public void Migrate()
    {
        dbContext.Database.EnsureCreated();
        logger.LogInformation("Database migration completed successfully.");
    }
}