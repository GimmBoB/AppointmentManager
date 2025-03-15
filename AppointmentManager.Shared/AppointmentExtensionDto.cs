namespace AppointmentManager.Shared;

public class AppointmentExtensionDto
{
    public Guid Id { get; init; }
    public Guid AppointmentId { get; init; }
    public string FilePath { get; init; } = string.Empty;
}