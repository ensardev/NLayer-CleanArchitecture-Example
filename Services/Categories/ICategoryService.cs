using App.Services.Categories.Create;
using App.Services.Categories.Update;

namespace App.Services.Categories;

public interface ICategoryService
{
    Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);
    Task<ServiceResult<List<CategoryResponse>>> GetAllAsync();
    Task<ServiceResult<CategoryWithProductsResponse>> GetCategoryWithProductsAsync(int id);
    Task<ServiceResult<List<CategoryWithProductsResponse>>> GetCategoryWithProductsAsync();
}
