namespace Shop.Core.Catalog;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int SortOrder { get; set; }

    public List<ProductModel> Models { get; set; } = [];
}
