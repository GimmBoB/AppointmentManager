using AppointmentManager.API.Database;
using AppointmentManager.API.Models;

namespace AppointmentManager.API.Repositories;

public class AppointmentCategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentCategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentCategory> AddAsync(AppointmentCategory category)
    {
        var result = (await _dbContext.AppointmentCategories.AddAsync(category)).Entity;
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<AppointmentCategory> UpdateAsync(AppointmentCategory category)
    {
        var result = _dbContext.AppointmentCategories.Update(category).Entity;
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public Task<List<AppointmentCategory>> GetAsync(CategorySearchFilter searchFilter)
    {
        var query = _dbContext.AppointmentCategories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchFilter.Name))
            query = query.Where(category => category.Name != null && category.Name.Trim().ToLower() ==
                searchFilter.Name.Trim().ToLower());

        var result = query.ToList();
        
        return Task.FromResult(result);
    }

    public async Task<AppointmentCategory?> GetByIdAsync(Guid id) =>
        await _dbContext.AppointmentCategories.FindAsync(id);

    public async Task DeleteAsync(AppointmentCategory category)
    {
        _dbContext.AppointmentCategories.Remove(category);

        await _dbContext.SaveChangesAsync();
    }
}