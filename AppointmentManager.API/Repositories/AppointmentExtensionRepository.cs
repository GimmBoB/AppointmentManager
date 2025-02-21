using AppointmentManager.API.Database;
using AppointmentManager.API.Models;

namespace AppointmentManager.API.Repositories;

public class AppointmentExtensionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentExtensionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentExtension> AddAsync(AppointmentExtension extension, CancellationToken ct)
    {
        var result = (await _dbContext.AppointmentExtensions.AddAsync(extension, ct)).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public async Task<AppointmentExtension> UpdateAsync(AppointmentExtension extension, CancellationToken ct)
    {
        var result = _dbContext.AppointmentExtensions.Update(extension).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public async Task<List<AppointmentExtension>> GetAsync(AppointmentExtensionSearchFilter searchFilter, CancellationToken _)
    {
        var query = _dbContext.AppointmentExtensions.AsQueryable();

        if (searchFilter.AppointmentIds.Any())
            query = query.Where(extension => searchFilter.AppointmentIds.Contains(extension.AppointmentId));

        return query.ToList();
    }
}