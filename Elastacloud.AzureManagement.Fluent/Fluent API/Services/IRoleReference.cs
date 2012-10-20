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
    /// Interface used to set the properties of the deployment
    /// </summary>
    public interface IRoleReference : IServiceCompleteActivity
    {
        /// <summary>
        /// Selects an individual role in the deployment by name
        /// </summary>
        IRoleActivity ForRole(string name);

        /// <summary>
        /// Selects another role in a deployment by name
        /// </summary>
        IRoleActivity AndRole(string name);

        /// <summary>
        /// Replaces the configuration of a role with a specific .cscfg file
        /// </summary>
        IRoleReference ReplaceConfiguration(string filename);

        /// <summary>
        /// Blocks and waits until all of the roles are running
        /// </summary>
        IRoleReference WaitUntilAllRoleInstancesAreRunning();
    }
}