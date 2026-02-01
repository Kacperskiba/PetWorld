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

    public async Task<IEnumerable<Product>> SearchAsync(string query)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(query) ||
                        p.Description.Contains(query) ||
                        p.TargetPet.Contains(query))
            .ToListAsync();
    }
}
