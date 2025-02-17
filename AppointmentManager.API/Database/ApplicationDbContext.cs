using AppointmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppointmentManager.API.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }
    
    public DatabaseFacade GetDatabase() => base.Database;
    public virtual DbSet<Admin> Admins { get; set; }
    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<AppointmentCategory> AppointmentCategories { get; set; }
    public virtual DbSet<AppointmentExtension> AppointmentExtensions { get; set; }
    public virtual DbSet<AppointmentTimeSlot> AppointmentTimeSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>(builder =>
        {
            builder.HasKey(admin => admin.Id);
        });

        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.HasKey(appointment => appointment.Id);
            builder
                .HasOne<AppointmentCategory>()
                .WithMany()
                .HasForeignKey(appointment => appointment.AppointmentCategoryId).OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany<AppointmentExtension>()
                .WithOne(extension => extension.Appointment)
                .HasForeignKey(extension => extension.AppointmentId);
        });
        
        // TODO complete schema
    }
}