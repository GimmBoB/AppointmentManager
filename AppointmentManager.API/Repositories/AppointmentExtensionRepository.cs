using AppointmentManager.API.Database;
using AppointmentManager.API.Models;
using AppointmentManager.Shared;

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

    public async Task<AppointmentExtension?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _dbContext.AppointmentExtensions.FindAsync(new object?[] { id }, cancellationToken: ct);
}