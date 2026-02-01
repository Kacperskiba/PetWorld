using AutoMapper;
using PetWorld.Application.DTOs;
using PetWorld.Domain.Entities;

namespace PetWorld.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForCtorParam("Category", opt => opt.MapFrom(src => src.Category.ToString()));

        CreateMap<ChatSession, ChatHistoryItemDto>()
            .ForCtorParam("Date", opt => opt.MapFrom(src => src.CreatedAt))
            .ForCtorParam("Question", opt => opt.MapFrom(src => src.UserQuestion))
            .ForCtorParam("Answer", opt => opt.MapFrom(src => src.FinalResponse));
    }
}
