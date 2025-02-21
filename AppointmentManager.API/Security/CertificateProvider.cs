using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AppointmentManager.API.config;

namespace AppointmentManager.API.Security;

public class CertificateProvider
{
    private readonly CertificateConfig _config;
    private readonly StoreLocation _storeLocation;
    private readonly StoreName _storeName;
    
    public CertificateProvider(CertificateConfig config)
    {
        _config = config;
        _storeLocation = StoreLocation.CurrentUser;
        _storeName = StoreName.My;
    }
    public X509Certificate2 GenerateSelfSigned(DateTimeOffset? notBefore = default, DateTimeOffset? notAfter = default)
    {
        // if the storage doesn't work on your system:
        // https://stackoverflow.com/questions/17840825/cryptographicexception-was-unhandled-system-cannot-find-the-specified-file
        
        var setNotBefore = notBefore ?? DateTimeOffset.Now.AddDays(-1);
        var setNotAfter = notAfter ?? DateTimeOffset.Now.AddYears(_config.LifetimeInYears);
                
        const int strength = 2048;

        using var rsa = new RSACryptoServiceProvider(strength);

        var certRequest = new CertificateRequest($"CN={_config.Subject}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));

        var generatedCert = certRequest.CreateSelfSigned(setNotBefore, setNotAfter);

        var isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        var pfxGeneratedCert = isOsx
            ? new X509Certificate2(generatedCert.Export(X509ContentType.Pfx), default(string),
                X509KeyStorageFlags.Exportable)
            : new X509Certificate2(generatedCert.Export(X509ContentType.Pfx), default(string),
                X509KeyStorageFlags.PersistKeySet);

        return pfxGeneratedCert;
    }
    public X509Certificate2? Load()
    {
        using var userStore = new X509Store(_storeName, _storeLocation);
        userStore.Open(OpenFlags.ReadOnly);
        if (userStore.IsOpen)
        {
            var collection = userStore.Certificates.Find(X509FindType.FindBySubjectName, _config.Subject, false);
            if (collection.Any())
            {
                var maxNotAfter = collection.Max(cert => cert.NotAfter);
                return collection.First(cert => cert.NotAfter.Equals(maxNotAfter));
            }
            userStore.Close();
        }

        return default;
    }


    public X509Certificate2Collection LoadCollection()
    {
        using var userStore = new X509Store(_storeName, _storeLocation);
        userStore.Open(OpenFlags.ReadOnly);
        if (userStore.IsOpen)
        {
            var collection = userStore.Certificates.Find(X509FindType.FindBySubjectName, _config.Subject, false);
            if (collection.Any())
                return collection;
            userStore.Close();
        }

        return new X509Certificate2Collection();
    }

    public void Save(X509Certificate2 certificate)
    {
        using var userStore = new X509Store(_storeName, _storeLocation);
        userStore.Open(OpenFlags.ReadWrite);
        userStore.Add(certificate);
        userStore.Close();
    }

    public void Delete(X509Certificate2 certificate)
    {
        using var userStore = new X509Store(_storeName, _storeLocation);
        userStore.Open(OpenFlags.ReadWrite);
        userStore.Remove(certificate);
        userStore.Close();
    }

    public void DeleteCollection(X509Certificate2Collection certificates)
    {
        using var userStore = new X509Store(_storeName, _storeLocation);
        userStore.Open(OpenFlags.ReadWrite);
        userStore.RemoveRange(certificates);
        userStore.Close();
    }
}