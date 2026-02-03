using PetWorld.Application.DTOs;

namespace PetWorld.Application.Interfaces;

public interface IChatHistoryService
{
    Task<IEnumerable<ChatHistoryDto>> GetAllHistoryAsync();
    Task<ChatHistoryDto> SaveChatHistoryAsync(ChatHistoryDto chatHistoryDto);
}
