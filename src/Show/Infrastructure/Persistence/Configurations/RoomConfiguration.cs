using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.HasKey(room => room.Id);
        builder
            .HasMany(room => room.Seats)
            .WithOne()
            .HasForeignKey(seat => seat.RoomId);
    }
}
