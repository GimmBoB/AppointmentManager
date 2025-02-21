﻿using AppointmentManager.API.config;
using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Database;
using AppointmentManager.API.Repositories;

namespace AppointmentManager.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppointmentManagerServices(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services.AddDbContext<ApplicationDbContext>();

        var dbConfig = new DatabaseConfiguration();
        var adminConfig = new AdminConfiguration(); 
        
        configurationManager.GetSection(nameof(DatabaseConfiguration)).Bind(dbConfig);
        configurationManager.GetSection(nameof(AdminConfiguration)).Bind(adminConfig);

        services.AddSingleton(dbConfig);
        services.AddSingleton(adminConfig);

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
        
        return services;
    }
}