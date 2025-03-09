namespace AppointmentManager.Shared;

public record TimeSlotSearchFilter(DayOfWeek? Day, FreeSlotSearchFilter? FreeSlots = default);