namespace Shop.Core.Catalog;

// Stock of one size of a product. Primary key: (ProductId, Size).
public class ProductSize
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // "S", "M", "L" or "42" for shoes
    public required string Size { get; set; }

    // 0 = unavailable
    public int Stock { get; set; }
}
