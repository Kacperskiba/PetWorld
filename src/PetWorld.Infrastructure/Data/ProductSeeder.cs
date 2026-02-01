using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Enums;

namespace PetWorld.Infrastructure.Data;

public static class ProductSeeder
{
    public static async Task SeedAsync(PetWorldDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var products = new List<Product>
        {
            new()
            {
                Name = "Royal Canin Adult Dog 15kg",
                Description = "Premium karma dla dorosłych psów. Pełnowartościowa, zbilansowana dieta wspierająca zdrowie i witalność.",
                Price = 289m,
                Category = ProductCategory.DogFood,
                TargetPet = "Dog",
                InStock = true
            },
            new()
            {
                Name = "Whiskas Adult Kurczak 7kg",
                Description = "Pełnowartościowa karma dla dorosłych kotów z pysznym kurczakiem. Wspiera zdrowe serce i ostre widzenie.",
                Price = 129m,
                Category = ProductCategory.CatFood,
                TargetPet = "Cat",
                InStock = true
            },
            new()
            {
                Name = "Tetra AquaSafe 500ml",
                Description = "Uzdatniacz wody do akwarium. Neutralizuje chlor i metale ciężkie, chroni błony śluzowe ryb.",
                Price = 45m,
                Category = ProductCategory.FishSupplies,
                TargetPet = "Fish",
                InStock = true
            },
            new()
            {
                Name = "Trixie Drapak XL 150cm",
                Description = "Duży drapak dla kotów z wieloma poziomami, domkami i zabawkami. Idealny dla aktywnych kotów.",
                Price = 399m,
                Category = ProductCategory.Accessories,
                TargetPet = "Cat",
                InStock = true
            },
            new()
            {
                Name = "Kong Classic Large",
                Description = "Wytrzymała zabawka dla dużych psów. Można napełnić smakołykami dla dodatkowej zabawy.",
                Price = 69m,
                Category = ProductCategory.Accessories,
                TargetPet = "Dog",
                InStock = true
            },
            new()
            {
                Name = "Ferplast Klatka dla chomika",
                Description = "Przestronna klatka dla chomika z wyposażeniem: kołowrotek, domek, poidło i karmnik.",
                Price = 189m,
                Category = ProductCategory.SmallPetSupplies,
                TargetPet = "Hamster",
                InStock = true
            },
            new()
            {
                Name = "Flexi Smycz automatyczna 8m",
                Description = "Automatyczna smycz taśmowa 8m dla psów do 50kg. Ergonomiczny uchwyt, solidna konstrukcja.",
                Price = 119m,
                Category = ProductCategory.Accessories,
                TargetPet = "Dog",
                InStock = true
            },
            new()
            {
                Name = "Brit Premium Kitten 8kg",
                Description = "Karma dla kociąt do 12 miesiąca życia. Bogata w białko, wspiera prawidłowy rozwój.",
                Price = 159m,
                Category = ProductCategory.CatFood,
                TargetPet = "Cat",
                InStock = true
            },
            new()
            {
                Name = "JBL ProFlora CO2 Set",
                Description = "Kompletny system nawożenia CO2 do akwarium. Wspiera bujny wzrost roślin wodnych.",
                Price = 549m,
                Category = ProductCategory.FishSupplies,
                TargetPet = "Fish",
                InStock = true
            },
            new()
            {
                Name = "Vitapol Siano dla królików 1kg",
                Description = "Wysokiej jakości siano łąkowe dla królików i gryzoni. Podstawa zdrowej diety.",
                Price = 25m,
                Category = ProductCategory.SmallPetSupplies,
                TargetPet = "Rabbit",
                InStock = true
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
