using Microsoft.Extensions.AI;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using System.ComponentModel;

namespace PetWorld.Infrastructure.Agents;

public class WriterAgentFactory : IWriterAgentFactory
{
    private readonly IChatClient _baseChatClient;
    private readonly IProductService _productService;

    public const string WriterSystemPrompt = @"Jesteś pomocnym asystentem sklepu PetWorld, który pomaga klientom w wyborze produktów dla ich zwierząt.

Twoim zadaniem jest:
1. Użyć dostępnych narzędzi do wyszukania odpowiednich produktów
2. Odpowiedzieć na pytanie klienta
3. Polecić odpowiednie produkty z katalogu
4. Dostarczyć pomocne porady dotyczące zwierząt

WAŻNE: Najpierw użyj narzędzi do wyszukania produktów, a następnie odpowiedz klientowi.

Odpowiedz po polsku, w sposób przyjazny i pomocny. Na końcu odpowiedzi wymień polecane produkty w formacie:
POLECANE PRODUKTY: [nazwa produktu 1], [nazwa produktu 2], ...";

    public const string CriticSystemPrompt = @"Jesteś krytykiem odpowiedzi w systemie AI dla sklepu PetWorld.
ZASADA AKCEPTACJI: Bądź wyrozumiały! Akceptuj odpowiedzi, które są 'wystarczająco dobre'.

AKCEPTUJ odpowiedź (approved=true) jeśli:
- Odpowiada na pytanie klienta (nawet częściowo)
- Zawiera jakiekolwiek produkty z katalogu
- Jest napisana po polsku i jest zrozumiała
- Nie zawiera błędnych lub wymyślonych informacji

ODRZUĆ odpowiedź (approved=false) TYLKO jeśli:
- Całkowicie nie odpowiada na pytanie
- Nie zawiera żadnych produktów
- Zawiera poważne błędy lub nieprawdziwe informacje
- Jest obraźliwa lub nieprofesjonalna

Oceń odpowiedź i zwróć JSON w formacie:
{
  ""approved"": true/false,
  ""feedback"": ""krótkie wyjaśnienie oceny""
}

WAŻNE: Odpowiedz TYLKO w formacie JSON, bez dodatkowego tekstu.";

    public WriterAgentFactory(IChatClient baseChatClient, IProductService productService)
    {
        _baseChatClient = baseChatClient;
        _productService = productService;
    }

    public IChatClient CreateWriterAgent()
    {
        return new SystemPromptChatClient(_baseChatClient, WriterSystemPrompt);
    }

    public IChatClient CreateCriticAgent()
    {
        return new SystemPromptChatClient(_baseChatClient, CriticSystemPrompt);
    }

    public IList<AITool> CreateProductTools()
    {
        return new List<AITool>
        {
            AIFunctionFactory.Create(
                async ([Description("Słowo kluczowe do wyszukania (np. 'karma', 'zabawka', 'akwarium')")] string keyword) =>
                {
                    var products = await _productService.SearchProductsAsync(keyword);
                    return FormatProductsForAI(products);
                },
                "SearchProducts",
                "Wyszukuje produkty po słowie kluczowym w nazwie, opisie lub tagach"),

            AIFunctionFactory.Create(
                async ([Description("Typ zwierzęcia: 'Pies', 'Kot', 'Ryby', 'Gryzoń'")] string petType) =>
                {
                    var products = await _productService.GetByPetTypeAsync(petType);
                    return FormatProductsForAI(products);
                },
                "GetProductsByPetType",
                "Pobiera produkty dla określonego typu zwierzęcia"),

            AIFunctionFactory.Create(
                async ([Description("Kategoria produktu: 'Karma', 'Zabawki', 'Akcesoria', 'Akwarystyka', 'Klatki i transportery'")] string category) =>
                {
                    var products = await _productService.GetByCategoryAsync(category);
                    return FormatProductsForAI(products);
                },
                "GetProductsByCategory",
                "Pobiera produkty z określonej kategorii"),

            AIFunctionFactory.Create(
                async () =>
                {
                    var products = await _productService.GetAllProductsAsync();
                    return FormatProductsForAI(products);
                },
                "GetAllProducts",
                "Pobiera wszystkie dostępne produkty ze sklepu")
        };
    }

    private static string FormatProductsForAI(IEnumerable<ProductDto> products)
    {
        var productList = products.ToList();
        if (!productList.Any())
        {
            return "Nie znaleziono produktów spełniających kryteria.";
        }

        var formatted = productList.Select(p =>
            $"- {p.Name} ({p.Brand}) - {p.Price:F2} PLN - {p.Category} dla: {p.PetType}" +
            (p.InStock ? $" (dostępne: {p.StockQuantity} szt.)" : " (niedostępne)"));

        return string.Join("\n", formatted);
    }
}

/// <summary>
/// A chat client wrapper that prepends a system prompt to all messages.
/// </summary>
internal class SystemPromptChatClient : DelegatingChatClient
{
    private readonly string _systemPrompt;

    public SystemPromptChatClient(IChatClient innerClient, string systemPrompt)
        : base(innerClient)
    {
        _systemPrompt = systemPrompt;
    }

    public override Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var messagesWithSystem = PrependSystemPrompt(messages);
        return base.GetResponseAsync(messagesWithSystem, options, cancellationToken);
    }

    public override IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var messagesWithSystem = PrependSystemPrompt(messages);
        return base.GetStreamingResponseAsync(messagesWithSystem, options, cancellationToken);
    }

    private List<ChatMessage> PrependSystemPrompt(IEnumerable<ChatMessage> messages)
    {
        var messageList = messages.ToList();

        // Only add system prompt if not already present
        if (!messageList.Any(m => m.Role == ChatRole.System))
        {
            messageList.Insert(0, new ChatMessage(ChatRole.System, _systemPrompt));
        }

        return messageList;
    }
}
