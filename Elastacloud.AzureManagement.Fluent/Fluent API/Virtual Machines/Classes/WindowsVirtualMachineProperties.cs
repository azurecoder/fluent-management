using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes
{
    /// <summary>
    /// This contains the virtual machine properties to make the deployment
    /// </summary>
    public class WindowsVirtualMachineProperties : VirtualMachineProperties
    {
      
        /// <summary>
        /// Uses an existing cloud service instead of creating a new one
        /// </summary>
        public bool UseExistingCloudService { get; set; }
        /// <summary>
        /// the location where the cloud service should be deployed
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// The certificate used to manage the Service Management call 
        /// </summary>
        public X509Certificate2 Certificate { get; set; }
        /// <summary>
        /// The subscription id used to deploy to 
        /// </summary>
        public string SubscriptionId { get; set; }
        
    }
}
