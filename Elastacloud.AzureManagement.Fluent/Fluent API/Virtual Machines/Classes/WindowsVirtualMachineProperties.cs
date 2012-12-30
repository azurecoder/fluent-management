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
    public class WindowsVirtualMachineProperties
    {
        //private for the deployment to give the default value
        private string _deploymentName = null;
        // do the same with the role name if it doesn't exist
        private string _roleName = null;
        /// <summary>
        /// The storage account name for the VHDs
        /// </summary>
        public string StorageAccountName { get; set; }
        /// <summary>
        /// The cloud services name used to deploy to 
        /// </summary>
        public string CloudServiceName { get; set; }
        /// <summary>
        /// The name of the role being used
        /// </summary>
        public string RoleName
        {
            get { return _roleName ?? (_roleName = "fluentmanagement"); }
            set { _roleName = value; }
        }
        /// <summary>
        /// Uses an existing cloud service instead of creating a new one
        /// </summary>
        public bool UseExistingCloudService { get; set; }
        /// <summary>
        /// the location where the cloud service should be deployed
        /// </summary>
        public string Location { get; set; }
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
        /// <summary>
        /// The password for the windows Virtual machine
        /// </summary>
        public string AdministratorPassword { get; set; }
        /// <summary>
        /// A list of public endpoints for the virtual machine
        /// </summary>
        public Dictionary<string, int> PublicEndpoints { get; set; }
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
        public string DeploymentName { get { return _deploymentName ?? (_deploymentName = "fluentmanagement"); }
            set { _deploymentName = value; } }
    }
}
