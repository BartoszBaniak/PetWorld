using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Application.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var allProducts = await _productRepository.GetAllAsync();
        var searchLower = searchTerm.ToLower();

        return allProducts
            .Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower) ||
                p.Category.ToLower().Contains(searchLower) ||
                p.PetType.ToLower().Contains(searchLower) ||
                (p.Tags != null && p.Tags.ToLower().Contains(searchLower)))
            .Select(MapToDto);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            PetType = product.PetType,
            Brand = product.Brand,
            InStock = product.InStock,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            Tags = product.Tags
        };
    }
}
