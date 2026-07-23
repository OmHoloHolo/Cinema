using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<SeatEntity>
{
    public void Configure(EntityTypeBuilder<SeatEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
