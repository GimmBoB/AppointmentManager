namespace AppointmentManager.API.Models;

public record Appointment(
    Guid Id,
    string Name,
    string Email,
    string ExtraWishes,
    DateTime From,
    DateTime To,
    AppointmentStatus Status);
