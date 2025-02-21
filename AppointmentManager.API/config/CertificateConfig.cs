namespace AppointmentManager.API.config;

public class CertificateConfig
{
    public string Subject { get; set; } = "AppointmentManagerAPI";
    public int LifetimeInYears { get; set; } = 3;
}