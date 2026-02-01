using PetWorld.Domain.Entities;

namespace PetWorld.Domain.Interfaces;

public interface IChatSessionRepository
{
    Task<IEnumerable<ChatSession>> GetAllAsync();
    Task<ChatSession?> GetByIdAsync(int id);
    Task<ChatSession> CreateAsync(ChatSession session);
    Task UpdateAsync(ChatSession session);
    Task AddMessageAsync(ChatMessage message);
    Task<(IEnumerable<ChatSession> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? sortColumn, bool ascending);
}
