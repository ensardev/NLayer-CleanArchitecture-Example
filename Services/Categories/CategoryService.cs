using App.Repositories;
using App.Repositories.Categories;
using App.Services.Categories.Create;
using App.Services.Categories.Update;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        var isExistCategoryName = await categoryRepository.Where(c => c.Name == request.Name).AnyAsync();

        if (isExistCategoryName)
        {
            return ServiceResult<CreateCategoryResponse>.Failure($"Category with name '{request.Name}' already exists.", System.Net.HttpStatusCode.Conflict);
        }

        var newCategory = new Category { Name = request.Name };

        await categoryRepository.AddAsync(newCategory);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(new CreateCategoryResponse(newCategory.Id), $"api/category/{newCategory.Id}");

    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return ServiceResult.Failure($"Category with ID {id} not found.", System.Net.HttpStatusCode.NotFound);
        }

        var isExistCategoryName = await categoryRepository.Where(c => c.Name == request.Name && c.Id != category.Id).AnyAsync();

        if (isExistCategoryName)
        {
            return ServiceResult.Failure($"Category with name '{request.Name}' already exists.", System.Net.HttpStatusCode.Conflict);
        }

        category = mapper.Map(request, category);

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return ServiceResult.Failure($"Category with ID {id} not found.", System.Net.HttpStatusCode.NotFound);
        }

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CategoryResponse>>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAll().ToListAsync();

        var categoryResponses = mapper.Map<List<CategoryResponse>>(categories);

        return ServiceResult<List<CategoryResponse>>.Success(categoryResponses);
    }

    public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult<CategoryResponse>.Failure($"Category with ID {id} not found.", System.Net.HttpStatusCode.NotFound);
        }
        var categoryResponse = mapper.Map<CategoryResponse>(category);
        return ServiceResult<CategoryResponse>.Success(categoryResponse);
    }

    public async Task<ServiceResult<CategoryWithProductsResponse>> GetCategoryWithProductsAsync(int id)
    {
        var category = await categoryRepository.GetCategoryWithProductsAsync(id);

        if (category is null)
        {
            return ServiceResult<CategoryWithProductsResponse>.Failure($"Category with ID {id} not found.", System.Net.HttpStatusCode.NotFound);
        }

        var categoryResponse = mapper.Map<CategoryWithProductsResponse>(category);
        return ServiceResult<CategoryWithProductsResponse>.Success(categoryResponse);
    }

    public async Task<ServiceResult<List<CategoryWithProductsResponse>>> GetCategoryWithProductsAsync()
    {
        var categories = categoryRepository.GetCategoryWithProducts().ToListAsync();

        var categoryResponses = mapper.Map<List<CategoryWithProductsResponse>>(categories);

        return ServiceResult<List<CategoryWithProductsResponse>>.Success(categoryResponses);
    }

}
