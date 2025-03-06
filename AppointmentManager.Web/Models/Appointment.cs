﻿namespace AppointmentManager.Web.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ExtraWishes { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}