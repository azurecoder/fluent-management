using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes
{
    /// <summary>
    /// This contains the virtual machine properties to make the deployment
    /// </summary>
    public class VirtualMachineProperties
    {
        /// <summary>
        /// The storage account name for the VHDs
        /// </summary>
        public string StorageAccountName { get; set; }
        /// <summary>
        /// The cloud services name used to deploy to 
        /// </summary>
        public string CloudServiceName { get; set; }
        /// <summary>
        /// The VM size of the role instance
        /// </summary>
        public VmSize VmSize { get; set; }
        /// <summary>
        /// The certificate used to manage the Service Management call 
        /// </summary>
        public X509Certificate2 Certificate { get; set; }
        /// <summary>
        /// The subscription id used to deploy to 
        /// </summary>
        public string SubscriptionId { get; set; }
        /// <summary>
        /// The type of virtual machine being deployed
        /// </summary>
        public VirtualMachineTemplates VirtualMachineType { get; set; }
    }
}
