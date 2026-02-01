using PetWorld.Application.DTOs;

namespace PetWorld.Application.Interfaces;

public interface IAgentOrchestrationService
{
    Task<AgentResponseDto> GenerateResponseAsync(string userQuestion, IEnumerable<ProductDto> products);
}
