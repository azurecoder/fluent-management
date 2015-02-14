/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Xml.Linq;
namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// Interface used to specify the possible buyild activity tasks and settings 
    /// </summary>
    public interface IBuildActivity
    {
        /// <summary>
        /// Sets the cspkg endpoint in blob storage
        /// </summary>
        IHostedServiceActivity SetCspkgEndpoint(string uriEndpoint, string cscfgFilePath = null);

        /// <summary>
        /// Sets the cspkg endpoint in blob storage
        /// <param name="uriEndpoint">URI to the cloud package in blob storage.</param>
        /// <param name="configuration">The .cscfg configuration.</param>
        /// </summary>
        IHostedServiceActivity SetCspkgEndpoint(Uri uriEndpoint, XDocument configuration);

        /// <summary>
        /// Sets the build root directory
        /// </summary>
        IDefinitionActivity SetBuildDirectoryRoot(string directoryName);
    }
}