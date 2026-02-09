using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;
using System.Text.Json;

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
        var iterationsJson = chatHistoryDto.Iterations.Any()
            ? JsonSerializer.Serialize(chatHistoryDto.Iterations)
            : null;

        var chatHistory = ChatHistory.Create(
            chatHistoryDto.Question,
            chatHistoryDto.Answer,
            chatHistoryDto.IterationCount,
            chatHistoryDto.RecommendedProducts,
            iterationsJson);

        var saved = await _chatHistoryRepository.AddAsync(chatHistory);
        return MapToDto(saved);
    }

    private static ChatHistoryDto MapToDto(ChatHistory chatHistory)
    {
        var iterations = new List<IterationDetail>();
        if (!string.IsNullOrEmpty(chatHistory.IterationsJson))
        {
            try
            {
                iterations = JsonSerializer.Deserialize<List<IterationDetail>>(chatHistory.IterationsJson) ?? new();
            }
            catch
            {
                // If deserialization fails, return empty list
            }
        }

        return new ChatHistoryDto
        {
            Id = chatHistory.Id,
            Timestamp = chatHistory.Timestamp,
            Question = chatHistory.Question,
            Answer = chatHistory.Answer,
            IterationCount = chatHistory.IterationCount,
            RecommendedProducts = chatHistory.RecommendedProducts,
            Iterations = iterations
        };
    }
}
