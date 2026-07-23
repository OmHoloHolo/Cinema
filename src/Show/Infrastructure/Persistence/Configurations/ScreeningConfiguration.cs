using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence.Configurations;

public class ScreeningConfiguration : IEntityTypeConfiguration<ScreeningEntity>
{
    public void Configure(EntityTypeBuilder<ScreeningEntity> builder)
    {
        builder.HasKey(movie => movie.Id);
        builder.HasOne(screening => screening.Room)
            .WithMany()
            .HasForeignKey(screening => screening.RoomId);
        builder.HasOne(screening => screening.Movie)
            .WithMany()
            .HasForeignKey(screening => screening.MovieId);
    }
}
