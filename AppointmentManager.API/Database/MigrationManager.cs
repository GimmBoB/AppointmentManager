using AppointmentManager.API.config;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.API.Database;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        var scope = host.Services.CreateScope();
        var dbConfig = scope.ServiceProvider.GetRequiredService<DatabaseConfiguration>();

        ArgumentNullException.ThrowIfNull(dbConfig, nameof(DatabaseConfiguration));

        using var dbContext = GetDatabaseContext(scope);
        var database = dbContext.Database;
        database.Migrate();

        return host;
    }

    private static ApplicationDbContext GetDatabaseContext(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
}