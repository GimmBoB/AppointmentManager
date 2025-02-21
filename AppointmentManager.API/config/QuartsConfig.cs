namespace AppointmentManager.API.config;

public class QuartsConfig
{
    public ICollection<QuartsJob> Jobs { get; set; } = new List<QuartsJob>
        { new() { Name = "EnsureValidCertificateJob", WithCronSchedule = "0 0 9 ? * SUN *" } };
}