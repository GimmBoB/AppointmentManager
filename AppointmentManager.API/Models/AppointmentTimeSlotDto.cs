namespace AppointmentManager.API.Models;

public record AppointmentTimeSlotDto(
    Guid Id,
    Days Day,
    TimeSpan From,
    TimeSpan To);
