using App.Repositories;
using App.Repositories.Products;
using System.Net;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<ServiceResult<CreateProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };
        
        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<CreateProductResponse>.Success(new CreateProductResponse(product.Id));
    }

    public async Task<ServiceResult> UpdateProductAsync(int id, CreateProductRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Failure($"Product with ID {id} not found.", HttpStatusCode.NotFound);
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteProductAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Failure($"Product with ID {id} not found.", HttpStatusCode.NotFound);
        }

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);

        var productResponse = products.Select(p=> new ProductResponse(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return new ServiceResult<List<ProductResponse>>()
        {
            Data = productResponse
        };
    }

    public async Task<ServiceResult<ProductResponse>> GetProductByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            ServiceResult<ProductResponse>.Failure($"Product with ID {id} not found.", HttpStatusCode.NotFound);
        }
        
        var productResponse = new ProductResponse(product.Id, product.Name, product.Price, product.Stock);

        return ServiceResult<ProductResponse>.Success(productResponse!);
    }
}
