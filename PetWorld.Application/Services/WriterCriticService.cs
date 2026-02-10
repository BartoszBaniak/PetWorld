using Microsoft.Agents.AI;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using System.Text.Json;

namespace PetWorld.Application.Services;

public class WriterCriticService : IWriterCriticService
{
    private readonly ChatClientAgent _writerAgent;
    private readonly ChatClientAgent _criticAgent;
    private const int MaxIterations = 3;

    private static readonly JsonSerializerOptions CaseInsensitiveJson = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WriterCriticService(IWriterAgentFactory agentFactory)
    {
        _writerAgent = agentFactory.CreateWriterAgent();
        _criticAgent = agentFactory.CreateCriticAgent();
    }

    public async Task<WriterCriticResponse> ProcessQuestionAsync(string question)
    {
        var response = new WriterCriticResponse();
        var iterations = new List<IterationDetail>();

        string writerResponse = string.Empty;
        bool approved = false;
        int iterationCount = 0;
        string criticFeedback = string.Empty;

        // Writer-Critic Loop (max 3 iterations)
        while (!approved && iterationCount < MaxIterations)
        {
            iterationCount++;

            // Writer Agent generates response (tool calls handled automatically by the framework)
            writerResponse = await RunWriterAgentAsync(question, criticFeedback);

            // Critic Agent evaluates response
            var criticResult = await RunCriticAgentAsync(question, writerResponse);
            approved = criticResult.approved;
            criticFeedback = criticResult.feedback;

            iterations.Add(new IterationDetail
            {
                IterationNumber = iterationCount,
                WriterResponse = writerResponse,
                CriticApproved = approved,
                CriticFeedback = criticFeedback
            });

            if (approved)
            {
                break;
            }
        }

        response.FinalAnswer = writerResponse;
        response.IterationCount = iterationCount;
        response.Iterations = iterations;
        response.RecommendedProducts = ExtractRecommendedProducts(writerResponse);

        return response;
    }

    private async Task<string> RunWriterAgentAsync(string question, string previousFeedback)
    {
        var userMessage = string.IsNullOrEmpty(previousFeedback)
            ? question
            : $"{question}\n\n[UWAGA: Poprzednia odpowiedź została odrzucona z powodu: {previousFeedback}. Popraw swoją odpowiedź.]";

        var result = await _writerAgent.RunAsync(userMessage);
        return result.Text ?? string.Empty;
    }

    private async Task<(bool approved, string feedback)> RunCriticAgentAsync(string question, string writerResponse)
    {
        var userMessage = $"Pytanie klienta: {question}\n\nOdpowiedź do oceny:\n{writerResponse}";

        var criticResult = await _criticAgent.RunAsync(userMessage);
        var criticResponseText = criticResult.Text ?? "{}";

        try
        {
            var jsonStart = criticResponseText.IndexOf('{');
            var jsonEnd = criticResponseText.LastIndexOf('}') + 1;
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonText = criticResponseText.Substring(jsonStart, jsonEnd - jsonStart);
                var criticEvaluation = JsonSerializer.Deserialize<CriticEvaluation>(jsonText, CaseInsensitiveJson);
                return (criticEvaluation?.Approved ?? false, criticEvaluation?.Feedback ?? string.Empty);
            }
        }
        catch
        {
            // If parsing fails, default to not approved
        }

        return (false, "Nie można było ocenić odpowiedzi poprawnie.");
    }

    private static List<string> ExtractRecommendedProducts(string response)
    {
        var products = new List<string>();

        if (response.Contains("POLECANE PRODUKTY:", StringComparison.OrdinalIgnoreCase))
        {
            var startIndex = response.IndexOf("POLECANE PRODUKTY:", StringComparison.OrdinalIgnoreCase);
            var productsText = response.Substring(startIndex + "POLECANE PRODUKTY:".Length);

            var items = productsText.Split([',', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
            products.AddRange(items.Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)));
        }

        return products;
    }

    private class CriticEvaluation
    {
        public bool Approved { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
