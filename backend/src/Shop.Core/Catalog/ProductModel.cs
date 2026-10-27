namespace Shop.Core.Catalog;

// Catalog item shared by all its color variants (Product).
public class ProductModel
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    public List<string> Tags { get; set; } = [];

    // UTC
    public DateTime CreatedAt { get; set; }

    public List<Product> Variants { get; set; } = [];
}
