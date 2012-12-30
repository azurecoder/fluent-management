/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines
{
    /// <summary>
    /// Used to manage all aspects of a virtual macine
    /// </summary>
    public class VirtualMachineManager : ICertificateActivity, IVirtualMachineActivity, IVirtualMachineQuery, IVirtualMachineDeployment
    {
        internal VirtualMachineManager(string subscriptionId)
        {
            Properties = new WindowsVirtualMachineProperties {SubscriptionId = subscriptionId};
        }
        /// <summary>
        /// The properties of the virtual machine
        /// </summary>
        public WindowsVirtualMachineProperties Properties { get; set; }

        #region Implementation of ICertificateActivity

        /// <summary>
        /// Adds a certificate to the request given an X509 v3 certificate
        /// </summary>
        IVirtualMachineActivity ICertificateActivity.AddCertificate(X509Certificate2 certificate)
        {
            Properties.Certificate = certificate;
            return this;
        }

        /// <summary>
        /// Adds a .publishsettings file and extracts the certificate
        /// </summary>
        IVirtualMachineActivity ICertificateActivity.AddPublishSettingsFromFile(string path)
        {
            var settings = new PublishSettingsExtractor(path);
            Properties.Certificate = settings.GetCertificateFromFile();
            return this;
        }

        /// <summary>
        /// Adds a .publishsettings profile from a given body of Xml
        /// </summary>
        IVirtualMachineActivity ICertificateActivity.AddPublishSettingsFromXml(string xml)
        {
            Properties.Certificate = PublishSettingsExtractor.AddPublishSettingsToPersonalMachineStore(xml);
            return this;
        }

        /// <summary>
        /// Adds a certificate from the store 
        /// </summary>
        IVirtualMachineActivity ICertificateActivity.AddCertificateFromStore(string thumbprint)
        {
            Properties.Certificate = PublishSettingsExtractor.FromStore(thumbprint);
            return this;
        }

        #endregion

        #region Implementation of IVirtualMachineActivity

        /// <summary>
        /// Allows a query to be made against the virtual machines catalog
        /// </summary>
        /// <returns>An IVirtualMachineQuery interface</returns>
        IVirtualMachineQuery IVirtualMachineActivity.QueryVirtualMachineManagement()
        {
            return this;
        }

        /// <summary>
        /// Creates a virtual machine deployment
        /// </summary>
        /// <returns>An IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment IVirtualMachineActivity.CreateVirtualMachineDeployment()
        {
            return this;
        }

        #endregion

        #region Implementation of IVirtualMachineDeployment

        /// <summary>
        /// Selects the size of the VM to provision
        /// </summary>
        /// <param name="size">the size of the VM to provision</param>
        /// <returns>An IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment IVirtualMachineDeployment.WithVmOfSize(VmSize size)
        {
            Properties.VmSize = size;
            return this;
        }

        /// <summary>
        /// The type of deployment that is being made - SQL, AD, stored image, etc.
        /// </summary>
        /// <param name="templates">The deployment type</param>
        /// <returns>A IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment IVirtualMachineDeployment.WithDeploymentType(VirtualMachineTemplates templates)
        {
            Properties.VirtualMachineType = templates;
            return this;
        }

        /// <summary>
        /// The setter for the storage account to hold the VHDs for the data disk and the OS disk
        /// </summary>
        /// <param name="storageAccount">the storage account used to store the VHDs</param>
        /// <returns>A IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment IVirtualMachineDeployment.WithStorageAccountForVhds(string storageAccount)
        {
            Properties.StorageAccountName = storageAccount;
            return this;
        }

        /// <summary>
        /// The cloud service account which the virtual machine is being deployed to
        /// </summary>
        /// <param name="name">the name of the cloud service</param>
        /// <returns>A IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment IVirtualMachineDeployment.AddToExistingCloudServiceWithName(string name)
        {
            Properties.CloudServiceName = name;
            return this;
        }

        /// <summary>
        /// Used to deploy the virtual machine 
        /// </summary>
        void IVirtualMachineDeployment.Deploy()
        {
            var command = new CreateWindowsVirtualMachineDeploymentCommand(Properties)
                              {
                                  SubscriptionId = Properties.SubscriptionId,
                                  Certificate = Properties.Certificate
                              };
            command.Execute();
        }

        #endregion
    }
}