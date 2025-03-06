using AppointmentManager.API.Database;
using AppointmentManager.API.Models;
using AppointmentManager.Shared;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.API.Repositories;

public class AppointmentTimeSlotRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentTimeSlotRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentTimeSlot> AddAsync(AppointmentTimeSlot timeSlot, CancellationToken ct)
    {
        var result = (await _dbContext.AppointmentTimeSlots.AddAsync(timeSlot, ct)).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public async Task<AppointmentTimeSlot> UpdateAsync(AppointmentTimeSlot timeSlot, CancellationToken ct)
    {
        var result = _dbContext.AppointmentTimeSlots.Update(timeSlot).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return result;
    }

    public Task<List<AppointmentTimeSlot>> GetAsync(TimeSlotSearchFilter searchFilter, CancellationToken _)
    {
        var query = _dbContext.AppointmentTimeSlots.AsQueryable();

        if (searchFilter.Days.HasValue)
            query = query.Where(slot => slot.Day == searchFilter.Days);

        if (searchFilter.freeSlots is not null)
        {
            var appointments = _dbContext.Appointments.Where(appointment =>
                appointment.Status != AppointmentStatus.Canceled && searchFilter.freeSlots.To >= appointment.From &&
                searchFilter.freeSlots.From <= appointment.To).ToList();

            var from = appointments.Select(appointment => new TimeSpan(appointment.From.Hour, appointment.From.Minute,
                appointment.From.Second));
            var to = appointments.Select(appointment => new TimeSpan(appointment.To.Hour, appointment.To.Minute,
                appointment.To.Second));
            
            if (appointments.Any())
            {
                query = query.Where(slot => !from.Contains(slot.To) && !to.Contains(slot.To));
            }
        }

        var result = query.OrderBy(slot => slot.Day).ThenBy(slot => slot.From).ToList();
        
        return Task.FromResult(result);
    }

    public async Task<AppointmentTimeSlot?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _dbContext.AppointmentTimeSlots.FindAsync(new object?[] { id }, cancellationToken: ct);

    public async Task DeleteAsync(AppointmentTimeSlot timeSlot, CancellationToken ct)
    {
        _dbContext.AppointmentTimeSlots.Remove(timeSlot);

        await _dbContext.SaveChangesAsync(ct);
    }
}