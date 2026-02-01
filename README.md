# PetWorld

Aplikacja sklepu zoologicznego z inteligentnym asystentem AI, który pomaga klientom w wyborze produktów dla ich zwierząt.

## Opis

PetWorld to aplikacja webowa zbudowana w technologii Blazor Server (.NET 9). Główną funkcjonalnością jest chatbot AI, który:

- Odpowiada na pytania klientów po polsku
- Rekomenduje produkty z katalogu sklepu
- Wykorzystuje system dwóch agentów (Writer + Critic) do generowania wysokiej jakości odpowiedzi

## Architektura

Projekt wykorzystuje Clean Architecture:

```
src/
├── PetWorld.Domain          # Encje i interfejsy domenowe
├── PetWorld.Application     # Logika biznesowa, DTOs, serwisy
├── PetWorld.Infrastructure  # Baza danych, integracja z LLM
└── PetWorld.Presentation    # UI Blazor Server
```

## Wymagania

- Docker i Docker Compose

## Uruchomienie

```bash
git clone https://github.com/Kacperskiba/PetWorld.git
cd PetWorld
docker compose up
```

Aplikacja będzie dostępna pod adresem: http://localhost:5000

## Konfiguracja LLM

Aplikacja obsługuje różnych dostawców LLM kompatybilnych z API OpenAI. Konfiguracja znajduje się w `src/PetWorld.Presentation/appsettings.json`:

### Groq (domyślnie)

```json
"LLM": {
  "ApiKey": "gsk_...",
  "Model": "llama-3.3-70b-versatile",
  "Endpoint": "https://api.groq.com/openai/v1"
}
```

### OpenAI

```json
"LLM": {
  "ApiKey": "sk-...",
  "Model": "gpt-4o"
}
```

### Inni dostawcy (Together, Anyscale, itp.)

```json
"LLM": {
  "ApiKey": "...",
  "Model": "nazwa-modelu",
  "Endpoint": "https://api.provider.com/v1"
}
```

## Technologie

- .NET 9 / Blazor Server
- Entity Framework Core
- MySQL 8.4
- OpenAI SDK (kompatybilne z Groq, OpenAI i innymi)
- Docker
