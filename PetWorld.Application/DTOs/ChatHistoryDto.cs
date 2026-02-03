namespace PetWorld.Application.DTOs;

public class ChatHistoryDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string Pytanie { get; set; } = string.Empty;
    public string Odpowiedz { get; set; } = string.Empty;
    public int LiczbaIteracji { get; set; }
    public string? RecommendedProducts { get; set; }
}
