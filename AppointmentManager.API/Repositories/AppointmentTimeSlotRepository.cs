using AppointmentManager.API.Database;
using AppointmentManager.API.Models;

namespace AppointmentManager.API.Repositories;

public class AppointmentTimeSlotRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentTimeSlotRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentTimeSlot> AddAsync(AppointmentTimeSlot timeSlot)
    {
        var result = (await _dbContext.AppointmentTimeSlots.AddAsync(timeSlot)).Entity;
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<AppointmentTimeSlot> UpdateAsync(AppointmentTimeSlot timeSlot)
    {
        var result = _dbContext.AppointmentTimeSlots.Update(timeSlot).Entity;
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public Task<List<AppointmentTimeSlot>> GetAsync(TimeSlotSearchFilter searchFilter)
    {
        var query = _dbContext.AppointmentTimeSlots.AsQueryable();

        if (searchFilter.Days.HasValue)
            query = query.Where(slot => slot.Day == searchFilter.Days);

        var result = query.OrderBy(slot => slot.Day).ThenBy(slot => slot.From).ToList();
        
        return Task.FromResult(result);
    }

    public async Task<AppointmentTimeSlot?> GetByIdAsync(Guid id) => await _dbContext.AppointmentTimeSlots.FindAsync(id);

    public async Task DeleteAsync(AppointmentTimeSlot timeSlot)
    {
        _dbContext.AppointmentTimeSlots.Remove(timeSlot);

        await _dbContext.SaveChangesAsync();
    }
}