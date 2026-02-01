namespace PetWorld.Application.DTOs;

public record AgentResponseDto(
    string Response,
    int IterationCount,
    List<int> RecommendedProductIds
);
