namespace AppointmentManager.Web.Models;

public class Appointment
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? ExtraWishes { get; init; }
    public DateTime From { get; init; }
    public DateTime To { get; init; }
}