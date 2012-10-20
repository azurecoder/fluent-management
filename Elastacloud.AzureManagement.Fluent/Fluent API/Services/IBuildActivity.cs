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
    /// Interface used to specify the possible buyild activity tasks and settings 
    /// </summary>
    public interface IBuildActivity
    {
        /// <summary>
        /// Sets the cspkg endpoint in blob storage
        /// </summary>
        IBuildActivity SetCspkgEndpoint(string uriEndpoint);

        /// <summary>
        /// Sets the build root directory
        /// </summary>
        IBuildActivity SetBuildDirectoryRoot(string directoryName);

        /// <summary>
        /// A rebuild command issued which invokes the command 
        /// </summary>
        ICertificateActivity Rebuild();

        /// <summary>
        /// Using the existing build rather than keeping on building 
        /// </summary>
        ICertificateActivity UseExistingBuild();
    }
}