namespace AppointmentManager.API.Security;

public static class SeedCertificateStore
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var certificateProvider = serviceProvider.GetRequiredService<CertificateProvider>();

        var certs = certificateProvider.LoadCollection();

        if (certs.Any()) return;
            
        var cert = certificateProvider.GenerateSelfSigned();
        certificateProvider.Save(cert);
    }
}