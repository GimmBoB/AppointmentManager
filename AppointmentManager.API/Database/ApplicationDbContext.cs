using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppointmentManager.API.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions)
    {
        
    }
    
    public DatabaseFacade GetDatabase() => base.Database;
}