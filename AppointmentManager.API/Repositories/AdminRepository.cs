using AppointmentManager.API.Database;
using AppointmentManager.API.Models;

namespace AppointmentManager.API.Repositories;

public class AdminRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AdminRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Admin> UpdateAsync(Admin admin)
    {
        var result = _dbContext.Admins.Update(admin).Entity;
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<Admin?> GetByIdAsync(Guid id) => await _dbContext.Admins.FindAsync(id);
}