namespace Shop.Core.Catalog;

public class Color
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Value used in URLs, filters and events, e.g. "black"
    public required string Slug { get; set; }

    // Swatch color for the UI, e.g. "#111111"
    public required string Hex { get; set; }

    public List<Product> Products { get; set; } = [];
}
