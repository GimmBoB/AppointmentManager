using AppointmentManager.API.config;
using AppointmentManager.API.Database;

namespace AppointmentManager.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppointmentManagerServices(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services.AddScoped<IApplicationDbContext, SqlServerDbContext>();
        services.AddDbContext<SqlServerDbContext>();

        var dbConfig =  new DatabaseConfiguration();
        
        configurationManager.GetSection(nameof(DatabaseConfiguration)).Bind(dbConfig);

        services.AddSingleton(dbConfig);
        
        return services;
    }
}