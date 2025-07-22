using App.Repositories;
using App.Repositories.Products;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var anyProduct = await productRepository.Where(p => p.Name == request.Name).AnyAsync();

        if (anyProduct)
        {
            return ServiceResult<CreateProductResponse>.Failure($"Product with name '{request.Name}' already exists.", HttpStatusCode.Conflict);
        }

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/product/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
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

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId);

        if (product is null)
        {
            return ServiceResult.Failure($"Product with ID {request.ProductId} not found.", HttpStatusCode.NotFound);
        }

        product.Stock = request.Stock;

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Failure($"Product with ID {id} not found.", HttpStatusCode.NotFound);
        }

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetAllAsync()
    {
        var products = await productRepository.GetAll().ToListAsync();

        var productResponses = products.Select(p => new ProductResponse(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductResponse>>.Success(productResponses);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAll()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var productResponses = products.Select(p => new ProductResponse(p.Id, p.Name, p.Price, p.Stock)).ToList();
        return ServiceResult<List<ProductResponse>>.Success(productResponses);
    }

    public async Task<ServiceResult<ProductResponse?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            ServiceResult<ProductResponse>.Failure($"Product with ID {id} not found.", HttpStatusCode.NotFound);
        }

        var productResponse = new ProductResponse(product!.Id, product.Name, product.Price, product.Stock);

        return ServiceResult<ProductResponse>.Success(productResponse)!;
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);

        var productResponse = products.Select(p => new ProductResponse(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return new ServiceResult<List<ProductResponse>>()
        {
            Data = productResponse
        };
    }
}
