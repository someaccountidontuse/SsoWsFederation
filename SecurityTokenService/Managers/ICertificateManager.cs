namespace SecurityTokenService.Managers
{
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    public interface ICertificateManager
    {
        RSACryptoServiceProvider PrivateKey { get; }

        X509Certificate2 GetCertificate();
    }
}