using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Application.Services;

public class ChatHistoryService : IChatHistoryService
{
    private readonly IRepository<ChatHistory> _chatHistoryRepository;

    public ChatHistoryService(IRepository<ChatHistory> chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<IEnumerable<ChatHistoryDto>> GetAllHistoryAsync()
    {
        var histories = await _chatHistoryRepository.GetAllAsync();
        return histories.OrderByDescending(h => h.Data).Select(MapToDto);
    }

    public async Task<ChatHistoryDto> SaveChatHistoryAsync(ChatHistoryDto chatHistoryDto)
    {
        var chatHistory = new ChatHistory
        {
            Data = chatHistoryDto.Data,
            Pytanie = chatHistoryDto.Pytanie,
            Odpowiedz = chatHistoryDto.Odpowiedz,
            LiczbaIteracji = chatHistoryDto.LiczbaIteracji,
            RecommendedProducts = chatHistoryDto.RecommendedProducts
        };

        var saved = await _chatHistoryRepository.AddAsync(chatHistory);
        return MapToDto(saved);
    }

    private static ChatHistoryDto MapToDto(ChatHistory chatHistory)
    {
        return new ChatHistoryDto
        {
            Id = chatHistory.Id,
            Data = chatHistory.Data,
            Pytanie = chatHistory.Pytanie,
            Odpowiedz = chatHistory.Odpowiedz,
            LiczbaIteracji = chatHistory.LiczbaIteracji,
            RecommendedProducts = chatHistory.RecommendedProducts
        };
    }
}
