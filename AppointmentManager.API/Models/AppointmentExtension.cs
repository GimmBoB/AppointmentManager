using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManager.API.Models;

public class AppointmentExtension
{
    public Guid Id { get; init; }
    public Guid AppointmentId { get; init; }
    public string? FilePath { get; init; }
    [ForeignKey(nameof(AppointmentId))] public Appointment? Appointment { get; init; }
}
