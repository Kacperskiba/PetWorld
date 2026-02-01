namespace PetWorld.Application.DTOs;

public record ChatHistoryItemDto(
    int Id,
    DateTime Date,
    string Question,
    string Answer,
    int IterationCount
);
