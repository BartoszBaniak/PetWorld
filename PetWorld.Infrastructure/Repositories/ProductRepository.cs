using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Data;

namespace PetWorld.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly PetWorldDbContext _context;

    public ProductRepository(PetWorldDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        var searchLower = searchTerm.ToLower();

        return await _context.Products
            .Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower) ||
                p.Category.ToLower().Contains(searchLower) ||
                p.PetType.ToLower().Contains(searchLower) ||
                p.Brand.ToLower().Contains(searchLower) ||
                (p.Tags != null && p.Tags.ToLower().Contains(searchLower)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPetTypeAsync(string petType)
    {
        return await _context.Products
            .Where(p => p.PetType.ToLower() == petType.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetInStockAsync()
    {
        return await _context.Products
            .Where(p => p.InStock && p.StockQuantity > 0)
            .ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
