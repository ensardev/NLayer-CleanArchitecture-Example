using App.Repositories;
using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;
using App.Services.ExceptionHandlers;
using AutoMapper;
using App.Services.Products.UpdateStock;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var isExistProductName = await productRepository.Where(p => p.Name == request.Name).AnyAsync();

        if (isExistProductName)
        {
            return ServiceResult<CreateProductResponse>.Failure($"Product with name '{request.Name}' already exists.", HttpStatusCode.Conflict);
        }

        var product = mapper.Map<Product>(request);

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

        var isExistProductName = await productRepository.Where(p => p.Name == request.Name && p.Id != product.Id).AnyAsync();

        if (isExistProductName)
        {
            return ServiceResult.Failure($"Product with name '{request.Name}' already exists.", HttpStatusCode.Conflict);
        }

        product = mapper.Map(request, product);

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
        var productResponses = mapper.Map<List<ProductResponse>>(products);

        return ServiceResult<List<ProductResponse>>.Success(productResponses);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAll()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productResponses = mapper.Map<List<ProductResponse>>(products);

        return ServiceResult<List<ProductResponse>>.Success(productResponses);
    }

    public async Task<ServiceResult<ProductResponse?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult<ProductResponse>.Failure($"Product with ID {id} not found.", HttpStatusCode.NotFound);
        }
        var productResponse = mapper.Map<ProductResponse>(product);

        return ServiceResult<ProductResponse>.Success(productResponse)!;
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);
        var productResponse = mapper.Map<List<ProductResponse>>(products);

        return new ServiceResult<List<ProductResponse>>()
        {
            Data = productResponse
        };
    }
}
