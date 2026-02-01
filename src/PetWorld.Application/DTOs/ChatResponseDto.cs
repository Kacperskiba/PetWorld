namespace PetWorld.Application.DTOs;

public record ChatResponseDto(
    string Response,
    int IterationCount,
    IEnumerable<ProductDto> RecommendedProducts
);
