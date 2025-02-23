using AppointmentManager.API.Models;

namespace AppointmentManager.API.config;

public class AdminConfiguration
{
    public IEnumerable<Admin> Admins { get; set; } = new List<Admin>();
    public string SecretKey { get; set; } = string.Empty;
}