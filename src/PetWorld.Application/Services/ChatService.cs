using AutoMapper;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAgentOrchestrationService _agentService;
    private readonly IMapper _mapper;

    public ChatService(IUnitOfWork unitOfWork, IAgentOrchestrationService agentService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _agentService = agentService;
        _mapper = mapper;
    }

    public async Task<ChatResponseDto> ProcessQuestionAsync(ChatRequestDto request)
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        var session = new ChatSession
        {
            UserQuestion = request.Question,
            CreatedAt = DateTime.UtcNow
        };
        session = await _unitOfWork.ChatSessions.CreateAsync(session);

        await _unitOfWork.ChatSessions.AddMessageAsync(new ChatMessage
        {
            ChatSessionId = session.Id,
            Role = "user",
            Content = request.Question,
            Iteration = 0
        });

        var agentResponse = await _agentService.GenerateResponseAsync(request.Question, productDtos);

        session.FinalResponse = agentResponse.Response;
        session.IterationCount = agentResponse.IterationCount;
        session.IsCompleted = true;
        await _unitOfWork.ChatSessions.UpdateAsync(session);

        await _unitOfWork.ChatSessions.AddMessageAsync(new ChatMessage
        {
            ChatSessionId = session.Id,
            Role = "assistant",
            Content = agentResponse.Response,
            Iteration = agentResponse.IterationCount
        });

        await _unitOfWork.SaveChangesAsync();

        var recommendedProducts = productDtos
            .Where(p => agentResponse.RecommendedProductIds.Contains(p.Id))
            .ToList();

        return new ChatResponseDto(agentResponse.Response, agentResponse.IterationCount, recommendedProducts);
    }

    public async Task<IEnumerable<ChatHistoryItemDto>> GetHistoryAsync()
    {
        var sessions = await _unitOfWork.ChatSessions.GetAllAsync();
        return _mapper.Map<IEnumerable<ChatHistoryItemDto>>(sessions.OrderByDescending(s => s.CreatedAt));
    }

    public async Task<PaginatedResult<ChatHistoryItemDto>> GetHistoryPagedAsync(int page, int pageSize, string? sortColumn, bool ascending)
    {
        var (sessions, totalCount) = await _unitOfWork.ChatSessions.GetPagedAsync(page, pageSize, sortColumn, ascending);
        var items = _mapper.Map<IEnumerable<ChatHistoryItemDto>>(sessions);
        return new PaginatedResult<ChatHistoryItemDto>(items, totalCount, page, pageSize);
    }
}
