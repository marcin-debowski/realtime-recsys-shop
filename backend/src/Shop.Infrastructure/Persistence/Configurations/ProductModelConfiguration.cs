using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Catalog;

namespace Shop.Infrastructure.Persistence.Configurations
{
    public class ProductModelConfiguration : IEntityTypeConfiguration<ProductModel>
    {
        public void Configure(EntityTypeBuilder<ProductModel> b)
        {
            b.Property(m => m.Name).HasMaxLength(200);
            b.Property(m => m.Slug).HasMaxLength(150);
            b.HasIndex(m => m.Slug).IsUnique();

            // Dictionaries: a category or brand that still has models cannot be deleted
            b.HasOne(m => m.Category)
                .WithMany(c => c.Models)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(m => m.Brand)
                .WithMany(br => br.Models)
                .HasForeignKey(m => m.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
