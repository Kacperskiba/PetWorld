using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Data;

namespace PetWorld.Infrastructure.Repositories;

public class ChatSessionRepository : IChatSessionRepository
{
    private readonly PetWorldDbContext _context;

    public ChatSessionRepository(PetWorldDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatSession>> GetAllAsync()
    {
        return await _context.ChatSessions
            .Include(c => c.Messages)
            .ToListAsync();
    }

    public async Task<ChatSession?> GetByIdAsync(int id)
    {
        return await _context.ChatSessions
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ChatSession> CreateAsync(ChatSession session)
    {
        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateAsync(ChatSession session)
    {
        _context.ChatSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<ChatSession> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? sortColumn, bool ascending)
    {
        var query = _context.ChatSessions.Include(c => c.Messages).AsQueryable();

        query = sortColumn?.ToLowerInvariant() switch
        {
            "date" => ascending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
            "question" => ascending ? query.OrderBy(c => c.UserQuestion) : query.OrderByDescending(c => c.UserQuestion),
            "answer" => ascending ? query.OrderBy(c => c.FinalResponse) : query.OrderByDescending(c => c.FinalResponse),
            "iterationcount" => ascending ? query.OrderBy(c => c.IterationCount) : query.OrderByDescending(c => c.IterationCount),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
