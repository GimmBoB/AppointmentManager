namespace AppointmentManager.API.Models;

public class AppointmentTimeSlot
{
    public Guid Id { get; init; }
    public Days Day { get; init; }
    public TimeSpan From { get; init; }
    public TimeSpan To { get; init; }
}