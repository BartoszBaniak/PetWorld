using PetWorld.Domain.Entities;

namespace PetWorld.Domain.Interfaces;

public interface IChatHistoryRepository
{
    Task<IEnumerable<ChatHistory>> GetAllAsync();
    Task<ChatHistory?> GetByIdAsync(int id);
    Task<IEnumerable<ChatHistory>> GetRecentAsync(int count);
    Task<ChatHistory> AddAsync(ChatHistory chatHistory);
    Task DeleteAsync(int id);
}
