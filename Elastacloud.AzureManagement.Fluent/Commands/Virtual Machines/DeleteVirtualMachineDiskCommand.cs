/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    /// Creates a deployment for a virtual machine and allows some preconfigured defaults from the image gallery 
    /// </summary>
    internal class DeleteVirtualMachineDiskCommand : ServiceCommand
    {
        // DELETE https://management.core.windows.net/<subscription-id>/services/disks/<disk-name>
        /// <summary>
        /// {subscriptionID}/services/hostedservices/{serviceName}/deployments/{deploymentName}/Roles/{roleName}/DataDisks/{lun
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        internal DeleteVirtualMachineDiskCommand(string diskName)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "";
            ServiceType = "services";
            HttpCommand = String.Format("disks/{0}", diskName);
            HttpVerb = HttpVerbDelete;
        }

        /// <summary>
        /// The name of the disk to delete
        /// </summary>
        protected string DiskName { get; set; }
        /// <summary>
        /// The name of the cloud service to use
        /// </summary>
        public string CloudServiceName { get; set; }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "DeleteVirtualMachineDiskCommand";
        }
    }
}