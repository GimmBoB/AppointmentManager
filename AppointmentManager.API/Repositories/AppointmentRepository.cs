using AppointmentManager.API.Database;
using AppointmentManager.API.Models;
using AppointmentManager.Shared;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.API.Repositories;

public class AppointmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken ct)
    {
        var result = (await _dbContext.Appointments.AddAsync(appointment, ct)).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }
    
    public async Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken ct)
    {
        var result = _dbContext.Appointments.Update(appointment).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public Task<List<Appointment>> GetAsync(AppointmentSearchFilter searchFilter, CancellationToken _)
    {
        var query = _dbContext.Appointments.AsQueryable();
        
        query = query
            .Include(a => a.AppointmentExtensions)
            .Where(appointment =>
            searchFilter.To > appointment.From &&
            searchFilter.From < appointment.To);

        var result = query.ToList();

        return Task.FromResult(result);
    }

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _dbContext.Appointments
            .Include(a => a.AppointmentExtensions)
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken: ct);
    }
}