using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Data;

namespace PetWorld.Infrastructure.Repositories;

public class ChatHistoryRepository : IChatHistoryRepository
{
    private readonly PetWorldDbContext _context;

    public ChatHistoryRepository(PetWorldDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatHistory>> GetAllAsync()
    {
        return await _context.ChatHistories
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync();
    }

    public async Task<ChatHistory?> GetByIdAsync(int id)
    {
        return await _context.ChatHistories.FindAsync(id);
    }

    public async Task<IEnumerable<ChatHistory>> GetRecentAsync(int count)
    {
        return await _context.ChatHistories
            .OrderByDescending(h => h.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<ChatHistory> AddAsync(ChatHistory chatHistory)
    {
        await _context.ChatHistories.AddAsync(chatHistory);
        await _context.SaveChangesAsync();
        return chatHistory;
    }

    public async Task DeleteAsync(int id)
    {
        var chatHistory = await GetByIdAsync(id);
        if (chatHistory != null)
        {
            _context.ChatHistories.Remove(chatHistory);
            await _context.SaveChangesAsync();
        }
    }
}
