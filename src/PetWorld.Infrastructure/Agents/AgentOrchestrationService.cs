using System.ClientModel;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;

namespace PetWorld.Infrastructure.Agents;

public class AgentOrchestrationService : IAgentOrchestrationService
{
    private readonly AIAgent _writerAgent;
    private readonly AIAgent _criticAgent;
    private readonly string _productCatalogPlaceholder = "{{PRODUCT_CATALOG}}";

    private readonly string _writerInstructions;
    private readonly string _criticInstructions;

    public AgentOrchestrationService(IConfiguration configuration)
    {
        var apiKey = configuration["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq:ApiKey is not configured");
        var model = configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.groq.com/openai/v1")
        };

        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient(model);

        _writerInstructions = $"""
            Jesteś pomocnym asystentem sklepu zoologicznego PetWorld.
            Odpowiadasz na pytania klientów i rekomendujesz produkty z naszego katalogu.

            Katalog produktów:
            {_productCatalogPlaceholder}

            Zasady:
            1. Odpowiadaj przyjaźnie i pomocnie po polsku
            2. Rekomenduj produkty z katalogu, które pasują do pytania klienta
            3. Podawaj nazwy produktów i ceny
            4. Jeśli pytanie nie dotyczy produktów, odpowiedz ogólnie pomocnie
            5. Zawsze wspominaj o ID produktów które rekomendujesz w formacie [ID:X]
            """;

        _criticInstructions = $$"""
            Jesteś krytykiem oceniającym odpowiedzi asystenta sklepu zoologicznego.

            Katalog produktów:
            {{_productCatalogPlaceholder}}

            Oceń odpowiedź według kryteriów:
            1. Czy odpowiedź jest pomocna i odpowiada na pytanie klienta?
            2. Czy rekomendowane produkty pasują do pytania?
            3. Czy odpowiedź jest przyjazna i profesjonalna?
            4. Czy podano prawidłowe ceny produktów?

            Odpowiedz TYLKO w formacie JSON:
            {"approved": true/false, "feedback": "twój feedback jeśli approved=false"}
            """;

        _writerAgent = chatClient.AsAIAgent(
            instructions: _writerInstructions,
            name: "WriterAgent"
        );

        _criticAgent = chatClient.AsAIAgent(
            instructions: _criticInstructions,
            name: "CriticAgent"
        );
    }

    public async Task<AgentResponseDto> GenerateResponseAsync(string userQuestion, IEnumerable<ProductDto> products)
    {
        var state = new FlowState();
        var productCatalog = BuildProductCatalog(products);

        while (!state.IsApproved && state.IterationCount < state.MaxIterations)
        {
            state.IterationCount++;

            // Writer Agent generates response
            state.CurrentResponse = await GenerateWriterResponse(userQuestion, productCatalog, state);

            // Extract recommended product IDs from the response
            state.RecommendedProductIds = ExtractProductIds(state.CurrentResponse, products);

            // Critic Agent evaluates response
            var decision = await EvaluateCriticResponse(userQuestion, state.CurrentResponse, productCatalog);

            if (decision.Approved || state.IterationCount >= state.MaxIterations)
            {
                state.IsApproved = true;
            }
            else
            {
                state.CriticFeedback = decision.Feedback;
            }
        }

        return new AgentResponseDto(
            state.CurrentResponse,
            state.IterationCount,
            state.RecommendedProductIds
        );
    }

    private string BuildProductCatalog(IEnumerable<ProductDto> products)
    {
        var lines = products.Select(p =>
            $"[ID:{p.Id}] {p.Name} - {p.Price} zł - {p.Category} - dla: {p.TargetPet} - {(p.InStock ? "dostępny" : "niedostępny")}");
        return string.Join("\n", lines);
    }

    private async Task<string> GenerateWriterResponse(string userQuestion, string productCatalog, FlowState state)
    {
        var prompt = userQuestion;

        if (!string.IsNullOrEmpty(state.CriticFeedback))
        {
            prompt = $"Poprzednia odpowiedź wymagała poprawek. Feedback: {state.CriticFeedback}\n\nPytanie klienta: {userQuestion}";
        }

        // Create a new agent with the product catalog injected
        var instructions = _writerInstructions.Replace(_productCatalogPlaceholder, productCatalog);

        // Use dynamic agent creation with updated instructions
        var response = await RunAgentWithInstructions(_writerAgent, instructions, prompt);
        return response;
    }

    private async Task<CriticDecision> EvaluateCriticResponse(string userQuestion, string writerResponse, string productCatalog)
    {
        var prompt = $"Pytanie klienta: {userQuestion}\n\nOdpowiedź asystenta: {writerResponse}";

        var instructions = _criticInstructions.Replace(_productCatalogPlaceholder, productCatalog);
        var content = await RunAgentWithInstructions(_criticAgent, instructions, prompt);

        try
        {
            // Try to parse JSON from the response
            var jsonStart = content.IndexOf('{');
            var jsonEnd = content.LastIndexOf('}') + 1;
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var json = content.Substring(jsonStart, jsonEnd - jsonStart);
                var decision = JsonSerializer.Deserialize<CriticJsonResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new CriticDecision(decision?.Approved ?? true, decision?.Feedback ?? "");
            }
        }
        catch
        {
            // If parsing fails, approve the response
        }

        return new CriticDecision(true, "");
    }

    private async Task<string> RunAgentWithInstructions(AIAgent baseAgent, string instructions, string prompt)
    {
        // Since AIAgent instructions are set at creation time, we need to include
        // context in the prompt itself for dynamic content like product catalog
        var fullPrompt = $"[System Context]\n{instructions}\n\n[User Message]\n{prompt}";
        var response = await baseAgent.RunAsync(fullPrompt);
        return response.Text;
    }

    private List<int> ExtractProductIds(string response, IEnumerable<ProductDto> products)
    {
        var ids = new List<int>();
        var productList = products.ToList();

        // Look for [ID:X] patterns
        var matches = System.Text.RegularExpressions.Regex.Matches(response, @"\[ID:(\d+)\]");
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (int.TryParse(match.Groups[1].Value, out var id) && productList.Any(p => p.Id == id))
            {
                ids.Add(id);
            }
        }

        // Also match by product name if no IDs found
        if (ids.Count == 0)
        {
            foreach (var product in productList)
            {
                if (response.Contains(product.Name, StringComparison.OrdinalIgnoreCase))
                {
                    ids.Add(product.Id);
                }
            }
        }

        return ids.Distinct().ToList();
    }

    private record CriticDecision(bool Approved, string Feedback);
    private record CriticJsonResponse(bool Approved, string? Feedback);
}
