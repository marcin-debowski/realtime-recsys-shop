using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Catalog;

namespace Shop.Infrastructure.Persistence.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> b)
        {
            b.Property(br => br.Name).HasMaxLength(100);
            b.Property(br => br.Slug).HasMaxLength(100);
            b.HasIndex(br => br.Slug).IsUnique();
        }
    }
}
