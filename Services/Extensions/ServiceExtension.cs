using App.Services.Products;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
