using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// Contains all of the things that need to have a certificate activity associated with it 
    /// </summary>
    public interface ICertificateActivity
    {
        /// <summary>
        /// Used to enable a role for SSL
        /// </summary>
        ICertificateActivity EnableSslForRole(string name);

        /// <summary>
        /// Used to enable remote desktop 
        /// </summary>
        IRemoteDesktop EnableRemoteDesktopForRole(string name);

        /// <summary>
        /// Adds an X509v3 management certificate using the .NET object of the same name
        /// </summary>
        IServiceCertificate AddCertificate(X509Certificate2 certificate);

        /// <summary>
        /// Adds a certificate from a publish settings file given a valid path to the file
        /// </summary>
        IServiceCertificate AddPublishSettingsFromFile(string path);

        /// <summary>
        /// Adds a certificate from a publish settings file given a valid block of Xml
        /// </summary>
        IServiceCertificate AddPublishSettingsFromXml(string xml);

        /// <summary>
        /// Searches various local certificate stores given a thumbprint from the store Local/Current User and Personal/Root
        /// </summary>
        IServiceCertificate AddCertificateFromStore(string thumbprint);
    }
}