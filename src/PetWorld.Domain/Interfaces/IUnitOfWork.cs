namespace PetWorld.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IChatSessionRepository ChatSessions { get; }
    Task<int> SaveChangesAsync();
}
