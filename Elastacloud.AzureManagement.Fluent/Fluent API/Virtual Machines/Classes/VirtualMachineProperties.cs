/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes
{
    public abstract class VirtualMachineProperties
    {
        //private for the deployment to give the default value
        private string _deploymentName = null;
        // do the same with the role name if it doesn't exist
        private string _roleName = null;
        /// <summary>
        /// A list of public endpoints for the virtual machine
        /// </summary>
        public List<InputEndpoint> PublicEndpoints { get; set; }
        /// <summary>
        /// A custom template available for the subscription used to override the Azure default template
        /// </summary>
        public string CustomTemplateName { get; set; }
        /// <summary>
        /// a list of the data disks that the will be created for the 
        /// </summary>
        public List<DataVirtualHardDisk> DataDisks { get; set; }
        /// <summary>
        /// The name of the deployment being used
        /// </summary>
        public string DeploymentName
        {
            get { return _deploymentName ?? (_deploymentName = "fluentmanagement"); }
            set { _deploymentName = value; }
        }
        /// <summary>
        /// The name of the role being used
        /// </summary>
        public string RoleName
        {
            get { return _roleName ?? (_roleName = "fluentmanagement"); }
            set { _roleName = value; }
        }
        /// <summary>
        /// The storage account name for the VHDs
        /// </summary>
        public string StorageAccountName { get; set; }
        /// <summary>
        /// The type of virtual machine being deployed
        /// </summary>
        public VirtualMachineTemplates VirtualMachineType { get; set; }
        /// <summary>
        /// The VM size of the role instance
        /// </summary>
        public VmSize VmSize { get; set; }
        /// <summary>
        /// The password for the windows Virtual machine
        /// </summary>
        public string AdministratorPassword { get; set; }
        /// <summary>
        /// The cloud services name used to deploy to 
        /// </summary>
        public string CloudServiceName { get; set; }
        /// <summary>
        /// The virtual network subnet name which 
        /// </summary>
        public VirtualNetworkDescriptor VirtualNetwork { get; set; }
        /// <summary>
        /// The availability set used to create the virtual machines
        /// Infinitly useful for zookeeper!!
        /// </summary>
        public string AvailabilitySet { get; set; }
        /// <summary>
        /// The subnets that are allowed through permit and deny rules 
        /// </summary>
        public List<EndpointAclRule> EndpointAclRules { get; set; }
    }


}
