using Microsoft.Extensions.AI;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using System.Text.Json;

namespace PetWorld.Application.Services;

public class WriterCriticService : IWriterCriticService
{
    private readonly IChatClient _chatClient;
    private readonly IProductService _productService;
    private const int MaxIterations = 3;

    public WriterCriticService(IChatClient chatClient, IProductService productService)
    {
        _chatClient = chatClient;
        _productService = productService;
    }

    public async Task<WriterCriticResponse> ProcessQuestionAsync(string question)
    {
        var response = new WriterCriticResponse();
        var iterations = new List<IterationDetail>();

        // Get all products for context
        var products = await _productService.GetAllProductsAsync();
        var productsContext = JsonSerializer.Serialize(products);

        string writerResponse = string.Empty;
        bool approved = false;
        int iterationCount = 0;

        string criticFeedback = string.Empty;

        while (!approved && iterationCount < MaxIterations)
        {
            iterationCount++;

            // Writer Agent - generates response
            writerResponse = await GenerateWriterResponse(question, productsContext, criticFeedback);

            // Critic Agent - evaluates response
            var criticResult = await EvaluateCriticResponse(question, writerResponse, productsContext);
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

    private async Task<string> GenerateWriterResponse(string question, string productsContext, string previousFeedback)
    {
        var systemPrompt = $@"Jesteś pomocnym asystentem sklepu PetWorld, który pomaga klientom w wyborze produktów dla ich zwierząt.
Twoim zadaniem jest:
1. Odpowiedzieć na pytanie klienta
2. Polecić odpowiednie produkty z dostępnego katalogu
3. Dostarczyć pomocne porady dotyczące zwierząt

Dostępne produkty:
{productsContext}

{(string.IsNullOrEmpty(previousFeedback) ? "" : $"Poprzednia odpowiedź została odrzucona z powodu: {previousFeedback}\nPopraw swoją odpowiedź.")}

Odpowiedz po polsku, w sposób przyjazny i pomocny. Na końcu odpowiedzi wymień polecane produkty w formacie:
POLECANE PRODUKTY: [nazwa produktu 1], [nazwa produktu 2], ...";

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, question)
        };

        var completionResponse = await _chatClient.GetResponseAsync(messages, cancellationToken: CancellationToken.None);
        return completionResponse.Text ?? string.Empty;
    }

    private async Task<(bool approved, string feedback)> EvaluateCriticResponse(string question, string writerResponse, string productsContext)
    {
        var systemPrompt = $@"Jesteś krytykiem odpowiedzi w systemie AI dla sklepu PetWorld.
Twoim zadaniem jest ocena czy odpowiedź:
1. Odpowiada na pytanie klienta
2. Poleca odpowiednie produkty z katalogu
3. Jest pomocna i profesjonalna
4. Zawiera realistyczne i trafne rekomendacje

Dostępne produkty:
{productsContext}

Oceń odpowiedź i zwróć JSON w formacie:
{{
  ""approved"": true/false,
  ""feedback"": ""szczegółowe wyjaśnienie dlaczego została odrzucona (jeśli approved=false) lub pusta wartość""
}}";

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, $"Pytanie klienta: {question}\n\nOdpowiedź do oceny:\n{writerResponse}")
        };

        var completionResponse = await _chatClient.GetResponseAsync(messages, cancellationToken: CancellationToken.None);
        var criticResponseText = completionResponse.Text ?? "{}";

        try
        {
            // Extract JSON from response
            var jsonStart = criticResponseText.IndexOf('{');
            var jsonEnd = criticResponseText.LastIndexOf('}') + 1;
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonText = criticResponseText.Substring(jsonStart, jsonEnd - jsonStart);
                var criticEvaluation = JsonSerializer.Deserialize<CriticEvaluation>(jsonText);
                return (criticEvaluation?.Approved ?? false, criticEvaluation?.Feedback ?? string.Empty);
            }
        }
        catch
        {
            // If parsing fails, default to not approved
        }

        return (false, "Nie można było ocenić odpowiedzi poprawnie.");
    }

    private List<string> ExtractRecommendedProducts(string response)
    {
        var products = new List<string>();

        if (response.Contains("POLECANE PRODUKTY:", StringComparison.OrdinalIgnoreCase))
        {
            var startIndex = response.IndexOf("POLECANE PRODUKTY:", StringComparison.OrdinalIgnoreCase);
            var productsText = response.Substring(startIndex + "POLECANE PRODUKTY:".Length);

            var items = productsText.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
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
