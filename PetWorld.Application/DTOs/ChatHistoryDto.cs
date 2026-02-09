namespace PetWorld.Application.DTOs;

public class ChatHistoryDto
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int IterationCount { get; set; }
    public string? RecommendedProducts { get; set; }
}
