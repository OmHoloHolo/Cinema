using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Show.Infrastructure.Persistence.Models;

namespace Show.Infrastructure.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<MovieEntity>
{
    public void Configure(EntityTypeBuilder<MovieEntity> builder)
    {
        builder.HasKey(movie => movie.Id);
    }
}