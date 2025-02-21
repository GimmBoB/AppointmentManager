namespace AppointmentManager.API.Models;

public class AppointmentCategory
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
