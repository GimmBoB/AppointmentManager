namespace AppointmentManager.API.Models;

public class AppointmentTimeSlot
{
    public Guid Id { get; init; }
    public Days Day { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
}