using AppointmentManager.Shared;

namespace AppointmentManager.API.Models;

public record AppointmentDto(
        Guid Id,
        Guid CategoryId,
        string Name,
        string Email,
        string? ExtraWishes,
        DateTime From,
        DateTime To,
        AppointmentStatus Status, ICollection<AppointmentExtensionDto> AppointmentExtensions) : IEntityDto;