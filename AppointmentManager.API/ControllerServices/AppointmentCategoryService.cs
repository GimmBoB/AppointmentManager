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

    public async Task<ApiResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);

        if (category is null)
            return NotFoundApiResult.NotFound();

        return ItemApiResult<CategoryDto>.Succeeded(MapToDto(category));
    }

    public async Task<ApiResult> AddAsync(CategoryDto dto, CancellationToken ct)
    {
        var categories = await _repository.GetAsync(new CategorySearchFilter(dto.Name), ct);
        
        var errors = Validate(Guid.Empty, dto, categories);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        var category = await _repository.AddAsync(new AppointmentCategory
            { Name = dto.Name, Description = dto.Description }, ct);

        return ItemApiResult<CategoryDto>.Created(MapToDto(category));
    }

    public async Task<ApiResult> UpdateAsync(Guid id, CategoryDto dto, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);

        if (category is null)
            return NotFoundApiResult.NotFound();

        var categories = await _repository.GetAsync(new CategorySearchFilter(dto.Name), ct);
        
        var errors = Validate(id, dto, categories);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        category.Name = dto.Name;
        category.Description = dto.Description;

        var result = await _repository.UpdateAsync(category, ct);

        return ItemApiResult<CategoryDto>.Succeeded(MapToDto(result));
    }

    public async Task<ApiResult> GetAllAsync(CancellationToken ct)
    {
        var result = await _repository.GetAsync(new CategorySearchFilter(null), ct);

        return ItemApiResult<ICollection<CategoryDto>>.Succeeded(result.Select(MapToDto).ToList());
    }

    public async Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);

        if (category is null)
            return NotFoundApiResult.NotFound();
        
        await _repository.DeleteAsync(category, ct);
        
        return ApiResult.Succeeded();
    }

    private static ICollection<string> Validate(Guid id, CategoryDto dto, IEnumerable<AppointmentCategory> categories)
    {
        var errors = new List<string>();

        if (categories.Any(category => category.Id != id))
            errors.Add($"There is already a category with the name '{dto.Name}'");
        
        return errors;
    }
    
    private static CategoryDto MapToDto(AppointmentCategory category)
    {
        return new CategoryDto(category.Id, category.Name, category.Description);
    }
}