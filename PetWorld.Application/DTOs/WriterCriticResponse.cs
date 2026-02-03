namespace PetWorld.Application.DTOs;

public class WriterCriticResponse
{
    public string FinalAnswer { get; set; } = string.Empty;
    public int IterationCount { get; set; }
    public List<string> RecommendedProducts { get; set; } = new();
    public List<IterationDetail> Iterations { get; set; } = new();
}

public class IterationDetail
{
    public int IterationNumber { get; set; }
    public string WriterResponse { get; set; } = string.Empty;
    public bool CriticApproved { get; set; }
    public string CriticFeedback { get; set; } = string.Empty;
}
