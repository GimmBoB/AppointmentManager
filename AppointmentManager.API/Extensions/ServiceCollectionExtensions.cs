using AppointmentManager.API.config;
using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Database;
using AppointmentManager.API.Repositories;
using AppointmentManager.API.Security;

namespace AppointmentManager.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppointmentManagerServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>();

        var dbConfig = new DatabaseConfiguration();
        var adminConfig = new AdminConfiguration();
        var certificateConfig = new CertificateConfig();
        var quartsConfig = new QuartsConfig();
        
        configuration.GetSection(nameof(DatabaseConfiguration)).Bind(dbConfig);
        configuration.GetSection(nameof(AdminConfiguration)).Bind(adminConfig);
        configuration.GetSection(nameof(CertificateConfig)).Bind(certificateConfig);
        configuration.GetSection(nameof(QuartsConfig)).Bind(quartsConfig);

        services.AddSingleton(dbConfig);
        services.AddSingleton(adminConfig);
        services.AddSingleton(certificateConfig);
        services.AddSingleton(quartsConfig);

        services.AddScoped<AdminService>();
        services.AddScoped<AdminRepository>();

        services.AddScoped<AppointmentTimeSlotService>();
        services.AddScoped<AppointmentTimeSlotRepository>();

        services.AddScoped<AppointmentCategoryService>();
        services.AddScoped<AppointmentCategoryRepository>();

        services.AddScoped<AppointmentService>();
        services.AddScoped<AppointmentRepository>();

        services.AddScoped<AppointmentExtensionService>();
        services.AddScoped<AppointmentExtensionRepository>();

        services.AddSingleton<CertificateProvider>();
        
        return services;
    }
}