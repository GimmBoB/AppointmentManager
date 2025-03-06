namespace AppointmentManager.Shared;

public record TimeSlotSearchFilter(DayOfWeek? Days, FreeSlotSearchFilter? freeSlots = default);