using PetWorld.Application.DTOs;

namespace PetWorld.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<ProductDto>> GetByPetTypeAsync(string petType);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
}
