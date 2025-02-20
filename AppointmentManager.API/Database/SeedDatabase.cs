using AppointmentManager.API.config;
using AppointmentManager.API.Models;
using AppointmentManager.API.Security;
using AppointmentManager.API.Utilities;

namespace AppointmentManager.API.Database;

public static class SeedDatabase
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var adminConfiguration = serviceProvider.GetRequiredService<AdminConfiguration>();

        await context.Database.EnsureCreatedAsync();
        
        var admins = new List<Admin>();
        foreach (var admin in adminConfiguration.Admins)
        {
            var exists = context.Admins.Any(a => a.Id == admin.Id);

            if (!exists)
            {
                ArgumentNullException.ThrowIfNull(admin.Password);
                ArgumentNullException.ThrowIfNull(admin.Name);
                ArgumentNullException.ThrowIfNull(admin.Email);
                if (!RegExUtil.IsValidEmail(admin.Email))
                    throw new ArgumentException($"'{admin.Email}' is not valid.");

                admins.Add(new Admin
                {
                    Id = admin.Id,
                    Name = admin.Name,
                    Email = admin.Email,
                    Password = StringCipher.Encrypt(admin.Password, adminConfiguration.SecretKey)
                });
            }
        }

        context.Admins.AddRange(admins);
        await context.SaveChangesAsync();
    }
}