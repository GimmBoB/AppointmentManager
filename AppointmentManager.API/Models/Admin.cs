namespace AppointmentManager.API.Models;

public class Admin
{
    public Guid Id { get; init; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}