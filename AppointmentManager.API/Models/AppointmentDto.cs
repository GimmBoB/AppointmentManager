﻿
// TODO bitches müssen Foto hochladen
namespace AppointmentManager.API.Models;

public record AppointmentDto(
        Guid Id,
        string Name,
        string Email,
        string ExtraWishes,
        DateTime From,
        DateTime To,
        AppointmentStatus Status);