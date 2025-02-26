namespace AppointmentManager.API.config;

public class SmtpConfig
{
    public string FromMail { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AdminMail { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public int MailSendRetries { get; set; }
    public int MillisecondsTillRetry { get; set; }
}