namespace PetWorld.Application.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    string TargetPet,
    bool InStock
);
