using Microsoft.EntityFrameworkCore;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence;

public class ShowDbContext(DbContextOptions<ShowDbContext> options) : DbContext(options)
{
    public DbSet<MovieEntity> Movies => Set<MovieEntity>();
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<ScreeningEntity> Screenings => Set<ScreeningEntity>();
    public DbSet<SeatEntity> Seats => Set<SeatEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShowDbContext).Assembly);
    }
}