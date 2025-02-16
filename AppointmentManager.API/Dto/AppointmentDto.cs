using AppointmentManager.API.Models;

// TODO bitches müssen Foto hochladen
namespace AppointmentManager.API.Dto;

public record AppointmentDto(
        string Name,
        string Email,
        string ExtraWishes,
        DateTime From,
        DateTime To,
        AppointmentStatus Status);