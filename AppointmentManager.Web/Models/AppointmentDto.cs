using AppointmentManager.Shared;

namespace AppointmentManager.Web.Models;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ExtraWishes { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public AppointmentStatus Status { get; set; }
    
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }

    public ICollection<AppointmentExtensionDto> AppointmentExtensions { get; set; } = Array.Empty<AppointmentExtensionDto>();
}