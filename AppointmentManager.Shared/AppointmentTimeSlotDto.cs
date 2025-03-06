namespace AppointmentManager.Shared;

public record AppointmentTimeSlotDto(
    Guid Id,
    DayOfWeek Day,
    TimeSpan From,
    TimeSpan To) : IEntityDto;
