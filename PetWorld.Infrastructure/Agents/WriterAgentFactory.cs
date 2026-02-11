using Microsoft.Agents.AI;
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
NIE opisuj co robisz przed wywołaniem narzędzi - po prostu wywołaj narzędzia bez komentarza, a dopiero po otrzymaniu wyników napisz odpowiedź.

Odpowiedz po polsku, w sposób przyjazny i pomocny. Na końcu odpowiedzi wymień polecane produkty w formacie:
POLECANE PRODUKTY: [nazwa produktu 1], [nazwa produktu 2], ...";

    public const string CriticSystemPrompt = @"Jesteś recenzentem odpowiedzi w systemie AI dla sklepu PetWorld.
Twoja rola to sprawdzenie, czy odpowiedź jest poprawna i pomocna dla klienta.

AKCEPTUJ odpowiedź (approved=true) jeśli:
- Odpowiedź odnosi się do pytania klienta
- Zawiera co najmniej 1 produkt z katalogu
- Jest napisana po polsku w przyjaznym tonie
- Nie zawiera ewidentnie błędnych informacji

ODRZUĆ odpowiedź (approved=false) TYLKO jeśli:
- Odpowiedź jest kompletnie nie na temat lub nie odpowiada na pytanie
- Nie zawiera żadnych produktów z katalogu
- Zawiera ewidentne błędy merytoryczne
- Jest napisana w innym języku niż polski

W feedbacku podaj wskazówkę co poprawić.

Oceń odpowiedź i zwróć JSON w formacie:
{
  ""approved"": true/false,
  ""feedback"": ""krótkie wyjaśnienie""
}

WAŻNE: Odpowiedz TYLKO w formacie JSON, bez dodatkowego tekstu.";

    public WriterAgentFactory(IChatClient baseChatClient, IProductService productService)
    {
        _baseChatClient = baseChatClient;
        _productService = productService;
    }

    public ChatClientAgent CreateWriterAgent()
    {
        var tools = CreateProductTools();

        return new ChatClientAgent(
            _baseChatClient,
            name: "WriterAgent",
            description: "Asystent sklepu PetWorld generujący odpowiedzi dla klientów",
            instructions: WriterSystemPrompt,
            tools: tools);
    }

    public ChatClientAgent CreateCriticAgent()
    {
        return new ChatClientAgent(
            _baseChatClient,
            name: "CriticAgent",
            description: "Krytyk oceniający jakość odpowiedzi",
            instructions: CriticSystemPrompt);
    }

    private IList<AITool> CreateProductTools()
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
