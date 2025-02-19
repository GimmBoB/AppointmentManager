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
        _dbContext.Admins.Update(admin);
        await _dbContext.SaveChangesAsync();

        return admin;
    }

    public async Task<Admin?> GetByIdAsync(Guid id) => await _dbContext.Admins.FindAsync(id);
}