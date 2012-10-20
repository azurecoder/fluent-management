/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.SqlAzure
{
    /// <summary>
    /// Used to add a Sql Azure Server
    /// </summary>
    public interface ISqlAzureServer
    {
        /// <summary>
        /// Adds a new Sql Azure Server instance at a specific location
        /// </summary>
        ISqlCertificateActivity AddNewServer(string location);

        /// <summary>
        /// Deletes a Sql Azure server instance in the cloud
        /// </summary>
        /// <param name="name">The given name of the Sql Azure server</param>
        /// <returns>An ICertificateActivity interface</returns>
        ISqlCertificateActivity DeleteServer(string name);

        /// <summary>
        /// Gets a handle to an existing Sql Azure instance with name {name}
        /// </summary>
        ISqlCertificateActivity UseSqlAzureServerWithName(string name);
    }
}