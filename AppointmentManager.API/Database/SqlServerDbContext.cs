using AppointmentManager.API.config;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.API.Database;

public class SqlServerDbContext : ApplicationDbContext
{
    private readonly DatabaseConfiguration _configuration;
    
    public SqlServerDbContext
    (DbContextOptions<SqlServerDbContext> dbContextOptions, DatabaseConfiguration configuration) : base(dbContextOptions)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(_configuration.ConnectionString);

        optionsBuilder.UseSqlServer(_configuration.ConnectionString);
    }
}