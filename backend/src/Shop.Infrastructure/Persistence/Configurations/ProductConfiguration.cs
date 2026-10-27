using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Catalog;

namespace Shop.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.Property(p => p.Slug).HasMaxLength(150);
            b.HasIndex(p => p.Slug).IsUnique();

            b.Property(p => p.Price).HasPrecision(10, 2);

            // A model cannot have the same color twice
            b.HasIndex(p => new { p.ModelId, p.ColorId }).IsUnique();

            // Color variants make no sense without their model
            b.HasOne(p => p.Model)
                .WithMany(m => m.Variants)
                .HasForeignKey(p => p.ModelId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(p => p.Color)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
