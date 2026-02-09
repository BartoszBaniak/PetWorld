namespace PetWorld.Domain.Entities;

public class ChatHistory : BaseEntity
{
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public string Question { get; private set; } = string.Empty;
    public string Answer { get; private set; } = string.Empty;
    public int IterationCount { get; private set; }
    public string? RecommendedProducts { get; private set; }

    private ChatHistory() { }

    private ChatHistory(
        string question,
        string answer,
        int iterationCount,
        string? recommendedProducts)
    {
        Timestamp = DateTime.UtcNow;
        Question = question;
        Answer = answer;
        IterationCount = iterationCount;
        RecommendedProducts = recommendedProducts;
    }

    public static ChatHistory Create(
        string question,
        string answer,
        int iterationCount,
        string? recommendedProducts = null)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("Question is required", nameof(question));

        if (string.IsNullOrWhiteSpace(answer))
            throw new ArgumentException("Answer is required", nameof(answer));

        if (iterationCount < 1)
            throw new ArgumentException("Iteration count must be at least 1", nameof(iterationCount));

        return new ChatHistory(question, answer, iterationCount, recommendedProducts);
    }
}
