using AutoMapper;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string query)
    {
        var products = await _unitOfWork.Products.SearchAsync(query);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(p => p.Category.ToString()).Distinct().OrderBy(c => c);
    }

    public async Task<IEnumerable<string>> GetTargetPetsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(p => p.TargetPet).Distinct().OrderBy(t => t);
    }
}
