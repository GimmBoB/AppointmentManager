using AppointmentManager.API.Models;

namespace AppointmentManager.API.config;

public class AdminConfiguration
{
    public IEnumerable<Admin> Admins { get; set; } = new List<Admin>();
    public string SecretKey { get; set; } = "LVT3PyA7bXP#3q$YlFj2YOhiu";
}