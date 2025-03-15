using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManager.Shared;

public class AppointmentExtension
{
    public Guid Id { get; init; }
    public Guid AppointmentId { get; init; }
    public string FilePath { get; init; } = string.Empty;
    [ForeignKey(nameof(AppointmentId))] public virtual Appointment? Appointment { get; init; }
}
