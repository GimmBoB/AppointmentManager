using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppointmentManager.API.Database;

public interface IApplicationDbContext: IApplicationDbSetContext, IDisposable
{
    public DatabaseFacade GetDatabase();
}