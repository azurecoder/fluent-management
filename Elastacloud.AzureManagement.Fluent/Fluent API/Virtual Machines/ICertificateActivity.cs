/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines
{
    /// <summary>
    /// A replica of the subscriptions manager certificate activity interface - these interfaces need to be consolitdate now!
    /// </summary>
    public interface ICertificateActivity
    {
        /// <summary>
        /// Adds a certificate to the request given an X509 v3 certificate
        /// </summary>
        IVirtualMachineActivity AddCertificate(X509Certificate2 certificate);

        /// <summary>
        /// Adds a .publishsettings file and extracts the certificate
        /// </summary>
        IVirtualMachineActivity AddPublishSettingsFromFile(string path);

        /// <summary>
        /// Adds a .publishsettings profile from a given body of Xml
        /// </summary>
        IVirtualMachineActivity AddPublishSettingsFromXml(string xml);

        /// <summary>
        /// Adds a certificate from the store 
        /// </summary>
        IVirtualMachineActivity AddCertificateFromStore(string thumbprint);
    }
}