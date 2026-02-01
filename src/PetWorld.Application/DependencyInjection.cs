using Microsoft.Extensions.DependencyInjection;
using PetWorld.Application.Interfaces;
using PetWorld.Application.Mapping;
using PetWorld.Application.Services;

namespace PetWorld.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
