namespace AppointmentManager.Web.Models;

public class Timeslot
{
    public Guid Id { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
}