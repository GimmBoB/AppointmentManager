using AppointmentManager.Shared;

namespace AppointmentManager.API.Models;

public record AppointmentTimeSlotDto(
    Guid Id,
    DayOfWeek Day,
    TimeSpan From,
    TimeSpan To) : IEntityDto;
