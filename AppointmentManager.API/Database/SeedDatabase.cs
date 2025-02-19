using AppointmentManager.API.config;
using AppointmentManager.API.Models;
using AppointmentManager.API.Security;

namespace AppointmentManager.API.Database;

public static class SeedDatabase
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var adminConfiguration = serviceProvider.GetRequiredService<AdminConfiguration>();

        await context.Database.EnsureCreatedAsync();
        
        if (context.Admins.Any())
            return;
            
        var admins = new List<Admin>();
        foreach (var admin in adminConfiguration.Admins)
        {
            admins.Add(new Admin
            {
                Id = admin.Id,
                Name = admin.Name,
                Email = admin.Email,
                Password = StringCipher.Encrypt(admin.Password!, adminConfiguration.SecretKey)
            });
        }
        
        context.Admins.AddRange(admins);
        await context.SaveChangesAsync();
    }
}