namespace Shop.Core.Catalog;

// A ProductModel in one color. Its Id is the productId used in events and recommendations (ADR-0005, ADR-0006).
public class Product
{
    public int Id { get; set; }

    public int ModelId { get; set; }
    public ProductModel Model { get; set; } = null!;

    public int ColorId { get; set; }
    public Color Color { get; set; } = null!;

    // e.g. "oversized-hoodie-black", used in /p/{slug}
    public required string Slug { get; set; }

    public decimal Price { get; set; }

    // Gallery order; imageIndex in image_interaction events points into this list
    public List<string> ImageUrls { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public List<ProductSize> Sizes { get; set; } = [];
}
