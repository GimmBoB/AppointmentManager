using AppointmentManager.API.Database;
using AppointmentManager.API.Models;
using AppointmentManager.Shared;

namespace AppointmentManager.API.Repositories;

public class AppointmentCategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentCategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentCategory> AddAsync(AppointmentCategory category, CancellationToken ct)
    {
        var result = (await _dbContext.AppointmentCategories.AddAsync(category, ct)).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public async Task<AppointmentCategory> UpdateAsync(AppointmentCategory category, CancellationToken ct)
    {
        var result = _dbContext.AppointmentCategories.Update(category).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public Task<List<AppointmentCategory>> GetAsync(CategorySearchFilter searchFilter, CancellationToken _)
    {
        var query = _dbContext.AppointmentCategories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchFilter.Name))
            query = query.Where(category => category.Name.Trim().ToLower() ==
                                            searchFilter.Name.Trim().ToLower());

        var result = query.ToList();
        
        return Task.FromResult(result);
    }

    public async Task<AppointmentCategory?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _dbContext.AppointmentCategories.FindAsync(new object?[] { id }, cancellationToken: ct);

    public async Task DeleteAsync(AppointmentCategory category, CancellationToken ct)
    {
        _dbContext.AppointmentCategories.Remove(category);

        await _dbContext.SaveChangesAsync(ct);
    }
}