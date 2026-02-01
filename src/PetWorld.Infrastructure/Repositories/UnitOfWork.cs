using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Data;

namespace PetWorld.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PetWorldDbContext _context;

    public IProductRepository Products { get; }
    public IChatSessionRepository ChatSessions { get; }

    public UnitOfWork(PetWorldDbContext context, IProductRepository products, IChatSessionRepository chatSessions)
    {
        _context = context;
        Products = products;
        ChatSessions = chatSessions;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
