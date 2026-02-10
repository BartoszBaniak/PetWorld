# PetWorld - AI-Powered Pet Store

PetWorld jest sklepem internetowym oferującym produkty dla zwierząt. Klienci mogą zadawać pytania o produkty za pośrednictwem czatu, a system AI pomaga im znaleźć odpowiednie produkty i udziela porad.

## Architektura

Projekt wykorzystuje **Onion Architecture** z następującymi warstwami:
- **PetWorld.Domain** - Encje domenowe, interfejsy
- **PetWorld.Application** - Logika biznesowa, serwisy, DTOs
- **PetWorld.Infrastructure** - Dostęp do danych, implementacje repozytoriów, agenci AI
- **PetWorld** (Blazor Server) - Warstwa prezentacji

## System Writer-Critic AI

Aplikacja wykorzystuje wzorzec Writer-Critic oparty na Microsoft Agent Framework i Microsoft.Extensions.AI:

- **Writer Agent** - Generuje odpowiedzi dla klientów, korzysta z narzędzi (tools) do wyszukiwania produktów w katalogu i rekomenduje produkty
- **Critic Agent** - Ocenia jakość odpowiedzi pod kątem konkretności, trafności rekomendacji, praktycznych porad i struktury. Zwraca JSON z decyzją (approved/rejected) oraz szczegółowym feedbackiem
- **Maksymalnie 3 iteracje** - Writer poprawia odpowiedź na podstawie feedbacku od Critica, aż odpowiedź zostanie zaakceptowana lub wyczerpie się limit iteracji

### Narzędzia Writer Agenta

Writer Agent ma dostęp do następujących narzędzi:
- `SearchProducts` - Wyszukiwanie produktów po słowie kluczowym
- `GetProductsByPetType` - Pobieranie produktów dla typu zwierzęcia (Pies, Kot, Ryby, Gryzoń)
- `GetProductsByCategory` - Pobieranie produktów z kategorii (Karma, Zabawki, Akcesoria, Akwarystyka, Klatki i transportery)
- `GetAllProducts` - Pobieranie wszystkich produktów

## Technologie

- **.NET 10.0**
- **Blazor Server** - UI
- **MySQL 8.0** - Baza danych
- **Entity Framework Core** - ORM
- **Microsoft.Extensions.AI** - Abstrakcja klientów AI
- **Google Gemini 2.5 Flash Lite** - Model AI
- **Docker & Docker Compose** - Konteneryzacja

## Wymagania

- Docker i Docker Compose
- Klucz API Google Gemini

## Instalacja i Uruchomienie

### 1. Sklonuj repozytorium

```bash
git clone <repository-url>
cd PetWorld
```

### 2. Skonfiguruj klucz API Gemini

Skopiuj plik `.env.example` do `.env` i uzupełnij klucz API:

```bash
cp .env.example .env
```

Edytuj plik `.env`:
```
GEMINI_API_KEY=your-gemini-api-key-here
```

Klucz API można uzyskać na: https://aistudio.google.com/apikey

Alternatywnie, możesz skonfigurować klucz w `PetWorld/appsettings.json`:
```json
{
  "Gemini": {
    "ApiKey": "your-gemini-api-key-here"
  }
}
```

### 3. Uruchom aplikację

```bash
docker compose up
```

### 4. Otwórz aplikację

Aplikacja będzie dostępna pod adresem: **http://localhost:5000**

## Strony

1. **Czat** (`/chat`, `/`) - Czat z asystentem AI
   - Sugestie szybkich pytań (produkty dla psa, kota, ryb)
   - Wyświetlanie odpowiedzi z liczbą iteracji Writer-Critic
   - Automatyczny zapis do historii

2. **Historia** (`/history`) - Historia rozmów
   - DataGrid (QuickGrid) z kolumnami: Data, Pytanie, Odpowiedź, Liczba iteracji
   - Sortowanie po dacie i liczbie iteracji

## Struktura bazy danych

### Products
- Id, Name, Description, Category, Price, PetType, Brand, InStock, StockQuantity, ImageUrl, Tags

### ChatHistories
- Id, Timestamp, Question, Answer, IterationCount, RecommendedProducts, IterationsJson

## Dodawanie produktów do katalogu

Produkty można dodać bezpośrednio do bazy danych MySQL lub przez migracje EF Core.

Przykład SQL:
```sql
INSERT INTO Products (Name, Description, Category, Price, PetType, Brand, InStock, StockQuantity, CreatedAt)
VALUES
('Karma dla psa Adult', 'Wysokiej jakości karma dla dorosłych psów', 'Karma', 89.99, 'Pies', 'Royal Canin', 1, 50, NOW()),
('Zabawka dla kota', 'Interaktywna zabawka dla kotów', 'Zabawki', 29.99, 'Kot', 'Trixie', 1, 100, NOW());
```

## Rozwój

### Lokalne uruchomienie bez Dockera

1. Zainstaluj MySQL i uruchom lokalnie
2. Zaktualizuj connection string w `appsettings.json`
3. Zastosuj migracje:
```bash
cd PetWorld
dotnet ef database update
```
4. Uruchom aplikację:
```bash
dotnet run
```

## Licencja

MIT
