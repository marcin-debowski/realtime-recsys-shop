using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Catalog;

namespace Shop.Infrastructure.Persistence.Configurations
{
    public class ColorConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> b)
        {
            b.Property(c => c.Name).HasMaxLength(50);
            b.Property(c => c.Slug).HasMaxLength(50);
            b.HasIndex(c => c.Slug).IsUnique();

            // "#111111"
            b.Property(c => c.Hex).HasMaxLength(7);
        }
    }
}
