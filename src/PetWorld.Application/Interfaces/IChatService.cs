using PetWorld.Application.DTOs;

namespace PetWorld.Application.Interfaces;

public interface IChatService
{
    Task<ChatResponseDto> ProcessQuestionAsync(ChatRequestDto request);
    Task<IEnumerable<ChatHistoryItemDto>> GetHistoryAsync();
    Task<PaginatedResult<ChatHistoryItemDto>> GetHistoryPagedAsync(int page, int pageSize, string? sortColumn, bool ascending);
}
