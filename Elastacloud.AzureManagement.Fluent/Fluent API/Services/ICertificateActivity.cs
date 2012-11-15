using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// Contains all of the things that need to have a certificate activity associated with it 
    /// </summary>
    public interface ICertificateActivity
    {
       
        /// <summary>
        /// Adds an X509v3 management certificate using the .NET object of the same name
        /// </summary>
        IDeploymentActivity AddCertificate(X509Certificate2 certificate);

        /// <summary>
        /// Adds a certificate from a publish settings file given a valid path to the file
        /// </summary>
        IDeploymentActivity AddPublishSettingsFromFile(string path);

        /// <summary>
        /// Adds a certificate from a publish settings file given a valid block of Xml
        /// </summary>
        IDeploymentActivity AddPublishSettingsFromXml(string xml);

        /// <summary>
        /// Searches various local certificate stores given a thumbprint from the store Local/Current User and Personal/Root
        /// </summary>
        IDeploymentActivity AddCertificateFromStore(string thumbprint);
    }
}