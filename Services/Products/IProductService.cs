﻿namespace App.Services.Products;

public interface IProductService
{
    Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<List<ProductResponse>>> GetAllAsync();
    Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize);
    Task<ServiceResult<ProductResponse?>> GetByIdAsync(int id);
    Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count);

}
