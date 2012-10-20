/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.SqlAzure
{
    /// <summary>
    /// Adds a certificate to the request to create a server/db
    /// </summary>
    public interface ISqlCertificateActivity
    {
        /// <summary>
        /// Adds an X509v3 certificate from a .NET object
        /// </summary>
        ISqlAzureSecurity AddCertificate(X509Certificate2 certificate);

        /// <summary>
        /// Adds a .publishsettings file and extracts the certificate given a path
        /// </summary>
        ISqlAzureSecurity AddPublishSettings(string path);

        /// <summary>
        /// Adds a certificate from a store given a valid thumbprint 
        /// </summary>
        ISqlAzureSecurity AddCertificateFromStore(string thumbprint);
    }
}