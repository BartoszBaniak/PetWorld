namespace PetWorld.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PetType { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public bool InStock { get; set; } = true;
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
}
