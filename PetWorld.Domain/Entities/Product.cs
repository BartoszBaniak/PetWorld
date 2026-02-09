namespace PetWorld.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string PetType { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public bool InStock { get; private set; } = true;
    public int StockQuantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Tags { get; private set; }

    private Product() { }

    private Product(
        string name,
        string description,
        string category,
        decimal price,
        string petType,
        string brand,
        bool inStock,
        int stockQuantity,
        string? imageUrl,
        string? tags)
    {
        Name = name;
        Description = description;
        Category = category;
        Price = price;
        PetType = petType;
        Brand = brand;
        InStock = inStock;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl;
        Tags = tags;
    }

    public static Product Create(
        string name,
        string description,
        string category,
        decimal price,
        string petType,
        string brand,
        bool inStock = true,
        int stockQuantity = 0,
        string? imageUrl = null,
        string? tags = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        return new Product(name, description, category, price, petType, brand, inStock, stockQuantity, imageUrl, tags);
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        InStock = quantity > 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));

        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
