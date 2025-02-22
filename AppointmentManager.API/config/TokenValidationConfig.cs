namespace AppointmentManager.API.config;

public class TokenValidationConfig
{
    public string ValidAudience { get; set; } = "https://localhost:5001";
    public string ValidIssuer { get; set; } = "https://localhost:5000";
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public int AccessLifetimeInSeconds { get; set; } = 3_000;
    public int RefreshLifetimeInSeconds { get; set; } = 30_000;
}