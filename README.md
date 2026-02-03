# PetWorld - AI-Powered Pet Store

PetWorld jest sklepem internetowym oferującym produkty dla zwierząt. Klienci mogą zadawać pytania o produkty za pośrednictwem czatu, a system AI pomaga im znaleźć odpowiednie produkty i udziela porad.

## Architektura

Projekt wykorzystuje **Onion Architecture** z następującymi warstwami:
- **PetWorld.Domain** - Encje domenowe, interfejsy
- **PetWorld.Application** - Logika biznesowa, serwisy, DTOs
- **PetWorld.Infrastructure** - Dostęp do danych, implementacje repozytoriów
- **PetWorld** (Blazor Server) - Warstwa prezentacji

## System Writer-Critic AI

Aplikacja wykorzystuje [Microsoft Agent Framework](https://github.com/microsoft/agent-framework) do implementacji systemu Writer-Critic:

- **Writer Agent** - Generuje odpowiedzi dla klientów i rekomenduje produkty
- **Critic Agent** - Ocenia jakość odpowiedzi (approved: true/false + feedback)
- **Maksymalnie 3 iteracje** - System może poprawiać odpowiedź do 3 razy

## Technologie

- **.NET 10.0**
- **Blazor Server** - UI
- **MySQL** - Baza danych
- **Entity Framework Core** - ORM
- **Microsoft Agent Framework** - System agentów AI
- **OpenAI GPT-4o** - Model AI
- **Docker & Docker Compose** - Konteneryzacja

## Wymagania

- Docker i Docker Compose
- Klucz API OpenAI lub Azure OpenAI

## Instalacja i Uruchomienie

### 1. Sklonuj repozytorium

```bash
git clone <repository-url>
cd PetWorld
```

### 2. Skonfiguruj klucz API OpenAI

Skopiuj plik `.env.example` do `.env` i uzupełnij klucz API:

```bash
cp .env.example .env
```

Edytuj plik `.env`:
```
OPENAI_API_KEY=your-actual-openai-api-key
```

Alternatywnie, możesz skonfigurować klucz w `PetWorld/appsettings.json`:
```json
{
  "OpenAI": {
    "ApiKey": "your-actual-openai-api-key"
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

1. **Czat** (`/chat`) - Czat z klientem
   - Pole tekstowe do wpisania pytania
   - Przycisk "Wyślij"
   - Wyświetlanie odpowiedzi + liczby iteracji Writer-Critic

2. **Historia** (`/history`) - Historia czatów
   - DataGrid z listą pytań i odpowiedzi
   - Kolumny: Data, Pytanie, Odpowiedź, Liczba iteracji, Polecane produkty

3. **System AI** (`/systemai`) - Informacje o systemie Writer-Critic
   - Opis Writer Agent
   - Opis Critic Agent
   - Przepływ pracy systemu

## Struktura bazy danych

### Products
- Id, Name, Description, Category, Price, PetType, Brand, InStock, StockQuantity, ImageUrl, Tags

### ChatHistories
- Id, Data, Pytanie, Odpowiedz, LiczbaIteracji, RecommendedProducts

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
