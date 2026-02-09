using Microsoft.Extensions.AI;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using System.Text.Json;

namespace PetWorld.Application.Services;

public class WriterCriticService : IWriterCriticService
{
    private readonly IWriterAgentFactory _agentFactory;
    private readonly IChatClient _writerAgent;
    private readonly IChatClient _criticAgent;
    private readonly IList<AITool> _productTools;
    private const int MaxIterations = 3;

    public WriterCriticService(IWriterAgentFactory agentFactory)
    {
        _agentFactory = agentFactory;
        _writerAgent = agentFactory.CreateWriterAgent();
        _criticAgent = agentFactory.CreateCriticAgent();
        _productTools = agentFactory.CreateProductTools();
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

            // Writer Agent generates response using tools
            writerResponse = await RunWriterWithToolsAsync(question, criticFeedback);

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

    private async Task<string> RunWriterWithToolsAsync(string question, string previousFeedback)
    {
        var userMessage = string.IsNullOrEmpty(previousFeedback)
            ? question
            : $"{question}\n\n[UWAGA: Poprzednia odpowiedź została odrzucona z powodu: {previousFeedback}. Popraw swoją odpowiedź.]";

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, userMessage)
        };

        var options = new ChatOptions
        {
            Tools = _productTools
        };

        // Loop to handle tool calls
        const int maxToolCalls = 5;
        int toolCallCount = 0;

        while (toolCallCount < maxToolCalls)
        {
            var chatResponse = await _writerAgent.GetResponseAsync(messages, options);

            // Check if there are tool calls in the response messages
            var toolCalls = chatResponse.Messages
                .SelectMany(m => m.Contents)
                .OfType<FunctionCallContent>()
                .ToList();

            if (!toolCalls.Any())
            {
                // No tool calls, return the response
                return chatResponse.Text ?? string.Empty;
            }

            // Add assistant messages with tool calls
            foreach (var msg in chatResponse.Messages)
            {
                messages.Add(msg);
            }

            // Execute tool calls and add results
            foreach (var toolCall in toolCalls)
            {
                var result = await ExecuteToolAsync(toolCall);
                messages.Add(new ChatMessage(ChatRole.Tool, [
                    new FunctionResultContent(toolCall.CallId, result)
                ]));
            }

            toolCallCount++;
        }

        // If we've exceeded max tool calls, get final response
        var finalResponse = await _writerAgent.GetResponseAsync(messages, options);
        return finalResponse.Text ?? string.Empty;
    }

    private async Task<string> ExecuteToolAsync(FunctionCallContent toolCall)
    {
        var tool = _productTools.OfType<AIFunction>().FirstOrDefault(t => t.Name == toolCall.Name);
        if (tool == null)
        {
            return $"Nieznane narzędzie: {toolCall.Name}";
        }

        try
        {
            var args = toolCall.Arguments != null
                ? new AIFunctionArguments(toolCall.Arguments)
                : null;
            var result = await tool.InvokeAsync(args);
            return result?.ToString() ?? "Brak wyników";
        }
        catch (Exception ex)
        {
            return $"Błąd podczas wykonywania narzędzia: {ex.Message}";
        }
    }

    private async Task<(bool approved, string feedback)> RunCriticAgentAsync(string question, string writerResponse)
    {
        var userMessage = $"Pytanie klienta: {question}\n\nOdpowiedź do oceny:\n{writerResponse}";

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, userMessage)
        };

        var response = await _criticAgent.GetResponseAsync(messages);
        var criticResponseText = response.Text ?? "{}";

        try
        {
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
