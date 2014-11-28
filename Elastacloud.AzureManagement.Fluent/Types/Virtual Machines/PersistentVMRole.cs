/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Represents a persistent Vm role
    /// </summary>
    public class PersistentVMRole : ICustomXmlSerializer
    {
        /// <summary>
        /// The name of the role being created
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// This is the type of role - currently only supports a PersistentVMRole
        /// </summary>
        public string RoleType
        {
            get { return "PersistentVMRole"; }
        }

        /// <summary>
        /// This is the name set which ties several virtual machine together such that they don't get placed on the same switch 
        /// </summary>
        public string AvailabilityNameSet { get; set; }

        /// <summary>
        /// Since there is a single VM instance per role this is referred to as role size
        /// </summary>
        public VmSize RoleSize { get; set; }

        /// <summary>
        /// This can either be Windows or Linux but not both - in future create an intermediate abstraction for both
        /// </summary>
        public ConfigurationSet OperatingSystemConfigurationSet { get; set; }

        /// <summary>
        /// The NetworkConfigurationSet that each Role must own
        /// </summary>
        public NetworkConfigurationSet NetworkConfigurationSet { get; set; }

        /// <summary>
        /// The hard disks associated with the virtual machine
        /// </summary>
        public DataVirtualHardDisks HardDisks { get; set; }
        /// <summary>
        /// Gets or sets the OS virtual hard disk 
        /// </summary>
        public OSVirtualHardDisk OSHardDisk { get; set; }
        /// <summary>
        /// The IP address used by the VM
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The name of the subnet which the virtual machine is a member of 
        /// </summary>
        public string SubnetName { get; set; }
        /// <summary>
        /// The name of the virtual network which the virtual machine should be a member of
        /// </summary>
        public string VirtualNetworkName { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var namer = new RandomAccountName();
            var element = new XElement(Namespaces.NsWindowsAzure + "Role",
                                       new XElement(Namespaces.NsWindowsAzure + "RoleName", RoleName),
                                       new XElement(Namespaces.NsWindowsAzure + "RoleType", RoleType));
            var configurationSets = new XElement(Namespaces.NsWindowsAzure + "ConfigurationSets", OperatingSystemConfigurationSet.GetXmlTree(),
                                                 NetworkConfigurationSet.GetXmlTree());
            element.Add(configurationSets);
            element.Add(HardDisks.GetXmlTree());
            element.Add(new XElement(Namespaces.NsWindowsAzure + "Label", Convert.ToBase64String(Encoding.UTF8.GetBytes(RoleName))));
            element.Add(OSHardDisk.GetXmlTree());
            element.Add(new XElement(Namespaces.NsWindowsAzure + "RoleSize", RoleSize.ToString()));
            if (AvailabilityNameSet != null)
                element.Add(Namespaces.NsWindowsAzure + "AvailabilitySetName", AvailabilityNameSet);
            if (SubnetName != null)
                element.Add(Namespaces.NsWindowsAzure + "SubnetNames",
                    new XElement(Namespaces.NsWindowsAzure + "SubnetName", SubnetName));
            return element;
        }

        #endregion

        /// <summary>
        /// Adds the default template for a Windows Virtual Machine
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static PersistentVMRole AddAdhocWindowsRoleTemplate(WindowsVirtualMachineProperties properties)
        {
            // build the windows configuration set
            var windows = new WindowsConfigurationSet
            {
                AdminPassword = properties.AdministratorPassword ?? "ElastaPassword101",
                ResetPasswordOnFirstLogon = true
            };
            return GetAdHocTemplate(properties, windows);
        }

        /// <summary>
        /// Adds the default template for a Windows Virtual Machine
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<PersistentVMRole> AddAdhocLinuxRoleTemplates(List<LinuxVirtualMachineProperties> properties)
        {
            return (from persistentVMRoleProperty in properties
                           let linux = new LinuxConfigurationSet()
                               {
                                   HostName = persistentVMRoleProperty.HostName, 
                                   DisableSshPasswordAuthentication = persistentVMRoleProperty.DisableSSHPasswordAuthentication, 
                                   UserName = persistentVMRoleProperty.UserName, 
                                   UserPassword = persistentVMRoleProperty.AdministratorPassword, 
                                   KeyPairs = persistentVMRoleProperty.KeyPairs, 
                                   PublicKeys = persistentVMRoleProperty.PublicKeys
                               }
                           select GetAdHocTemplate(persistentVMRoleProperty, linux)).ToList();
        }

        public static PersistentVMRole GetAdHocTemplate(VirtualMachineProperties properties, ConfigurationSet operatingSystemConfigurationSet)
        {
            // build the default endpoints 
            var inputEndpoints = new InputEndpoints();
            if (properties.PublicEndpoints == null)
                properties.PublicEndpoints = new List<InputEndpoint>();
            // add all of the endpoints 
            foreach (var endpoint in properties.PublicEndpoints)
            {
                inputEndpoints.AddEndpoint(endpoint);
            }

            if (operatingSystemConfigurationSet.ConfigurationSetType == ConfigurationSetType.WindowsProvisioningConfiguration)
            {
                if (properties.PublicEndpoints.All(endpoint => endpoint.Port != 3389))
                    inputEndpoints.AddEndpoint(InputEndpoint.GetDefaultRemoteDesktopSettings());
            }
            
            // add the endpoints collections to a network configuration set
            var network = new NetworkConfigurationSet
            {
                InputEndpoints = inputEndpoints
            };

            OSVirtualHardDisk osDisk = OSVirtualHardDisk.GetOSImageFromTemplate(properties);
            var disks = new DataVirtualHardDisks();
            if (properties.DataDisks != null)
            {
                for (int i = 0; i < properties.DataDisks.Count; i++)
                {
                    var label = properties.DataDisks[i].DiskLabel ?? "DataDisk" + i;
                    var name = properties.DataDisks[i].DiskName ?? "DataDisk" + i;
                    var size = properties.DataDisks[i].LogicalDiskSizeInGB < 30
                                   ? 30
                                   : properties.DataDisks[i].LogicalDiskSizeInGB;
                    var disk = DataVirtualHardDisk.GetDefaultDataDisk(properties.StorageAccountName, size, i, name, label);
                    disks.HardDiskCollection.Add(disk);
                }
            }
            return new PersistentVMRole
            {
                NetworkConfigurationSet = network,
                OperatingSystemConfigurationSet = operatingSystemConfigurationSet,
                RoleSize = properties.VmSize,
                RoleName = properties.RoleName,
                HardDisks = disks,
                OSHardDisk = osDisk
            };
        }
    }
}