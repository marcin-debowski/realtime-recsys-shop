using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Catalog;

namespace Shop.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.Property(c => c.Name).HasMaxLength(100);
            b.Property(c => c.Slug).HasMaxLength(100);
            b.HasIndex(c => c.Slug).IsUnique();
        }
    }
}