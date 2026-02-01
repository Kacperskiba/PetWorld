using PetWorld.Domain.Enums;

namespace PetWorld.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string TargetPet { get; set; } = string.Empty;
    public bool InStock { get; set; } = true;
}
