using AppointmentManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.API.Database;

public interface IApplicationDbSetContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentCategory> AppointmentCategories { get; set; }
    public DbSet<AppointmentExtension> AppointmentExtensions { get; set; }
    public DbSet<AppointmentTimeSlot> AppointmentTimeSlots { get; set; }
}