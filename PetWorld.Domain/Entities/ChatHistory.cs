namespace PetWorld.Domain.Entities;

public class ChatHistory : BaseEntity
{
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string Pytanie { get; set; } = string.Empty;
    public string Odpowiedz { get; set; } = string.Empty;
    public int LiczbaIteracji { get; set; }
    public string? RecommendedProducts { get; set; }
}
