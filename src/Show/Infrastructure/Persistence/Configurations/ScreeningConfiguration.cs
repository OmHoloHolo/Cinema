using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence.Configurations;

public class ScreeningConfiguration : IEntityTypeConfiguration<ScreeningEntity>
{
    public void Configure(EntityTypeBuilder<ScreeningEntity> builder)
    {
        builder.HasKey(movie => movie.Id);
        builder.HasOne<MovieEntity>()
            .WithMany()
            .HasForeignKey(screening => screening.Movie.Id);
        builder.HasOne<RoomEntity>()
            .WithMany()
            .HasForeignKey(screening => screening.Room.Id);
    }
}
