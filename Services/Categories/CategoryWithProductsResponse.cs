using App.Services.Products;

namespace App.Services.Categories;

public record CategoryWithProductsResponse(int Id, string Name, List<ProductResponse> Products);

