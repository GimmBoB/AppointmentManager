using System.Security.Cryptography.X509Certificates;
using AppointmentManager.API.Security;
using Quartz;

namespace AppointmentManager.API.QuartzJobs;

public class EnsureValidCertificateJob : IJob
{
    private readonly CertificateProvider _certificateProvider;

    public EnsureValidCertificateJob(CertificateProvider certificateProvider)
    {
        _certificateProvider = certificateProvider;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;

        var certs = _certificateProvider.LoadCollection();
        if (certs.Any())
        {
            var certsToDelete = certs.Where(cert => cert.NotAfter <= now).ToArray();

            if (certsToDelete.Any())
            {
                var certCollection = new X509Certificate2Collection(certsToDelete);
                _certificateProvider.DeleteCollection(certCollection);
            }
        }

        var currentCert = _certificateProvider.Load();
        if (currentCert == null || currentCert.NotAfter <= now.AddMonths(3))
        {
            var certToAdd = _certificateProvider.GenerateSelfSigned();
            _certificateProvider.Save(certToAdd);
        }

        return Task.CompletedTask;
    }
}