/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Blobs;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Services.Classes;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    /// <summary>
    /// Used to control a cloud service 
    /// </summary>
    public interface IServiceClient
    {
        /// <summary>
        /// Used to upload a service certificate to a cloud service if the thumbprint exists in the service
        /// </summary>
        /// <param name="thumbprint">The thumbprint of the certificate</param>
        /// <param name="password">The password used to export the key</param>
        void UploadServiceCertificate(string thumbprint, string password);
        /// <summary>
        /// Uploads a certificate given a valid service certificate and password
        /// </summary>
        /// <param name="certificate">The certificate being uploaded</param>
        /// <param name="password">The .pfx password for the certificate</param>
        /// <param name="includePrivateKey">Includes the private key</param>
        void UploadServiceCertificate(X509Certificate2 certificate, string password = "", bool includePrivateKey = false);
        /// <summary>
        /// Creates a new cloud service 
        /// </summary>
        /// <param name="location">the data centre location of the cloud service</param>
        /// <param name="description">The description of the cloud service</param>
        void CreateNewCloudService(string location, string description = "Fluent Management created cloud service");
        /// <summary>
        /// Used to delete the cloud service
        /// </summary>
        void DeleteCloudService();
        /// <summary>
        /// Used to delete the deployment in the respective cloud service spot
        /// </summary>
        /// <param name="slot">Either production or staging</param>
        void DeleteDeployment(DeploymentSlot slot = DeploymentSlot.Production);
        /// <summary>
        /// Updates a role instance count within a cloud services
        /// </summary>
        void UpdateRoleInstanceCount(string roleName, int count);

        /// <summary>
        /// Adds a remote desktop 
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="password">the password of the user</param>
        /// <param name="file">The Cscfg file</param>
        /// <returns>A service certificate to be uploaded to azure</returns>
        ServiceCertificate CreateServiceCertificateAndAddRemoteDesktop(string username, string password, ref CscfgFile file);
        /// <summary>
        /// Returns a list of roles for a given cloud service
        /// </summary>
        List<string> Roles { get; }
        /// <summary>
        /// Gets or sets the deployment slot for the cloud service
        /// </summary>
        DeploymentSlot Slot { get; set; }
        /// <summary>
        /// Starts all of the roles within a cloud service
        /// </summary>
        void Start();
        /// <summary>
        /// Stops all of the roles within a cloud service
        /// </summary>
        void Stop();
        /// <summary>
        /// Checks to see whether the cloud service name is available for use
        /// </summary>
        bool IsCloudServiceNameAvailable { get; }
        /// <summary>
        /// Gets a list of available locations for the subscription
        /// </summary>
        List<LocationInformation> AvailableLocations { get; }
        /// <summary>
        /// This is the account that will be used for any of the storage interactions 
        /// </summary>
        string DefaultStorageAccount { get; set; }
        /// <summary>
        /// Used to create a service certificate and export a .cer, .pfx and .pem to the file system 
        /// </summary>
        CertificateGenerator CreateServiceCertificateExportToFileSystem(string name, string password, string exportDirectory);
        /// <summary>
        /// Exports a service certificate to Windows Azure Storage
        /// </summary>
        CertificateGenerator CreateServiceCertificateExportToStorage(string name, string password, string storageAccountName, string container, string folder);
   
    }
}
