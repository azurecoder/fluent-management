/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// Used to add a service certificate to the config and upload 
    /// </summary>
    public interface IServiceCertificate
    {
        /// <summary>
        /// Adds a new service certificate for a particular role 
        /// </summary>
        IHostedServiceActivity GenerateAndAddServiceCertificate(string name);

        /// <summary>
        /// Adds a new service certificiate to the config and assumes has already been uploaded to an existing deployment
        /// </summary>
        IHostedServiceActivity UploadExistingServiceCertificate(string thumbprint, string password);

        /// <summary>
        /// Assumes that Generate of Upload Existing has been called and uses this given a name and thumbprint
        /// </summary>
        IHostedServiceActivity UsePreviouslyUploadedServiceCertificate(string name, string thumbprint);

        ///// <summary>
        ///// Continues without adding a service certificate to the deployment
        ///// </summary>
        ///// <returns></returns>
        //IHostedServiceActivity IgnoreServiceCertificate();
    }
}