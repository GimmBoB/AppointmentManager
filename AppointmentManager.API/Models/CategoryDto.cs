namespace AppointmentManager.API.Models;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description) : IEntityDto;