using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;

namespace AppointmentManager.API.ControllerServices;

public class AppointmentCategoryService
{
    private readonly AppointmentCategoryRepository _repository;

    public AppointmentCategoryService(AppointmentCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResult> AddAsync(CategoryDto dto)
    {
        var categories = await _repository.GetAsync(new CategorySearchFilter(dto.Name));
        
        var errors = Validate(Guid.Empty, dto, categories);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        var category = await _repository.AddAsync(new AppointmentCategory
            { Name = dto.Name, Description = dto.Description });

        return ItemApiResult<CategoryDto>.Succeeded(dto with { Id = category.Id });
    }

    public async Task<ApiResult> UpdateAsync(Guid id, CategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return ApiResult.NotFound();

        var categories = await _repository.GetAsync(new CategorySearchFilter(dto.Name));
        
        var errors = Validate(id, dto, categories);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _repository.UpdateAsync(category);

        return ItemApiResult<CategoryDto>.Succeeded(dto with { Id = id });
    }

    public async Task<ApiResult> GetAllAsync()
    {
        var result = await _repository.GetAsync(new CategorySearchFilter(null));

        return ItemApiResult<ICollection<AppointmentCategory>>.Succeeded(result);
    }

    public async Task<ApiResult> DeleteAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return ApiResult.NotFound();
        
        await _repository.DeleteAsync(category);
        
        return ApiResult.Succeeded();
    }

    private static ICollection<string> Validate(Guid id, CategoryDto dto, IEnumerable<AppointmentCategory> categories)
    {
        var errors = new List<string>();

        if (categories.Any(category => category.Id != id))
            errors.Add($"There is already a category with the name '{dto.Name}'");
        
        return errors;
    }
}