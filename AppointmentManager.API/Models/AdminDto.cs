using AppointmentManager.Shared;

namespace AppointmentManager.API.Models;

public record AdminDto(
    Guid Id,
    string Name,
    string Email) : IEntityDto;