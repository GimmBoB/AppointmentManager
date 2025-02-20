namespace AppointmentManager.API.Models;

public class AppointmentCategory
{
    public Guid Id { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
