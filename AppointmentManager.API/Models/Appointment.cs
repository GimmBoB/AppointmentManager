using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManager.API.Models;

public class Appointment
{
    public Guid Id { get; init; }
    public Guid AppointmentCategoryId { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? ExtraWishes { get; init; }
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public AppointmentStatus Status { get; init; }
    [ForeignKey(nameof(AppointmentCategoryId))] public virtual AppointmentCategory? AppointmentCategory { get; init; }
    public virtual ICollection<AppointmentExtension> AppointmentExtensions { get; init; } = new List<AppointmentExtension>();
}