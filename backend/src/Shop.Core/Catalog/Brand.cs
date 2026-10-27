namespace Shop.Core.Catalog;

public class Brand
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public List<ProductModel> Models { get; set; } = [];
}
