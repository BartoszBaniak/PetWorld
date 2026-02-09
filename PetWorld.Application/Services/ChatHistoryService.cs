using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Application.Services;

public class ChatHistoryService : IChatHistoryService
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public ChatHistoryService(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<IEnumerable<ChatHistoryDto>> GetAllHistoryAsync()
    {
        var histories = await _chatHistoryRepository.GetAllAsync();
        return histories.Select(MapToDto);
    }

    public async Task<ChatHistoryDto> SaveChatHistoryAsync(ChatHistoryDto chatHistoryDto)
    {
        var chatHistory = ChatHistory.Create(
            chatHistoryDto.Question,
            chatHistoryDto.Answer,
            chatHistoryDto.IterationCount,
            chatHistoryDto.RecommendedProducts);

        var saved = await _chatHistoryRepository.AddAsync(chatHistory);
        return MapToDto(saved);
    }

    private static ChatHistoryDto MapToDto(ChatHistory chatHistory)
    {
        return new ChatHistoryDto
        {
            Id = chatHistory.Id,
            Timestamp = chatHistory.Timestamp,
            Question = chatHistory.Question,
            Answer = chatHistory.Answer,
            IterationCount = chatHistory.IterationCount,
            RecommendedProducts = chatHistory.RecommendedProducts
        };
    }
}
