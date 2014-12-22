/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Services.Classes;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    public interface ILinuxVirtualMachineClient : IVirtualMachineClient
    {
        /// <summary>
        /// The service certificate that is deployed with the Linux VM
        /// </summary>
        byte[] ServiceCertificate { get; }
        /// <summary>
        /// Returns the properties of the associated virtual machine
        /// </summary>
        List<LinuxVirtualMachineProperties> Properties { get; set; }
        /// <summary>
        /// Gets thye configuration for the virtual machine
        /// </summary>
        List<PersistentVMRole> VirtualMachine { get; }

        /// <summary>
        /// Creates a new virtual machine from a gallery template
        /// </summary>
        /// <param name="properties">Can be any gallery template</param>
        /// <param name="cloudServiceName">The name of the cloud service - if it doesn't exist it will be created</param>
        /// <param name="location">Where the cloud service will be created if it doesn't exist</param>
        /// <param name="affinityGroup">Affinity group that this service will live in</param>
        IVirtualMachineClient CreateNewVirtualMachineDeploymentFromTemplateGallery(
            List<LinuxVirtualMachineProperties> properties, string cloudServiceName, ServiceCertificateModel serviceCertificate = null,
            string location = LocationConstants.NorthEurope, string affinityGroup = "");

        /// <summary>
        /// Lists all of the OS images assoiated with Linux that are public within the subscription
        /// </summary>
        List<ImageProperties> ListImages(string filter = "");
        /// <summary>
        /// Gets a list of virtual machine subnets that the vms in the cloud service belong to
        /// </summary>
        CloudServiceNetworking GetVirtualMachineSubnetCollection(string cloudServiceName);
    }
}
