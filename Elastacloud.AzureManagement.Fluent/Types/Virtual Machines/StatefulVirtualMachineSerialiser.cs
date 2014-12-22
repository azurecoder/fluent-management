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
using System.Xml.Linq;
using Microsoft.Data.Edm.Validation;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Used to serialise all XMl windows information whcih comes back from the command layer
    /// </summary>
    public class StatefulVirtualMachineSerialiser : StatefulSerialiser
    {
        private readonly XDocument _document;
        /// <summary>
        /// Used to construct the stateful serialiser
        /// </summary>
        public StatefulVirtualMachineSerialiser(XDocument doc)
        {
            _document = doc;
        }

        /// <summary>
        /// Gets a VM role for Windows which 
        /// </summary>
        public override List<PersistentVMRole> GetVmRoles()
        {
            var listVm = new List<PersistentVMRole>();
            // get all of the root properties
            var roots = _document.Descendants(Namespace + "RoleList").Elements(Namespace + "Role");
            // get the virtual network name if it exists
            var network = _document.Element(Namespace + "VirtualNetworkName");
            string virtualNetwork = null;
            if (network != null)
            {
                virtualNetwork = network.Value;
            }
            foreach (var root in roots)
            {
                var persistentVirtualMachine = new PersistentVMRole();

                if (root.Attribute(TypeNamespace + "type").Value != "PersistentVMRole")
                    throw new ApplicationException("non persistent vm role detected");

                persistentVirtualMachine.AvailabilityNameSet =
                    GetStringValue(root.Element(Namespace + "AvailabilitySetName"));
                persistentVirtualMachine.VirtualNetworkName = virtualNetwork;

                persistentVirtualMachine.RoleSize = GetEnumValue<VmSize>(root.Element(Namespace + "RoleSize"));
                persistentVirtualMachine.RoleName = GetStringValue(root.Element(Namespace + "RoleName"));
                // get the roleinstance from the list
                var roleInstance = _document.Descendants(Namespace + "RoleInstanceList").Elements(Namespace + "RoleInstance").FirstOrDefault(
                    a => a.Element(Namespace + "RoleName").Value == persistentVirtualMachine.RoleName);
                // add the ip address
                persistentVirtualMachine.IPAddress = GetStringValue(roleInstance.Element(Namespace + "IpAddress"));

                // get the networkconfiguration
                var configurationSets = root.Descendants(Namespace + "ConfigurationSet");
                foreach (var configurationSet in configurationSets)
                {
                    persistentVirtualMachine.NetworkConfigurationSet = GetNetworkConfigurationSet(configurationSet);
                }
                // get the hard disk
                var hardDisks = root.Element(Namespace + "DataVirtualHardDisks");
                persistentVirtualMachine.HardDisks = new DataVirtualHardDisks()
                    {
                        HardDiskCollection = GetHardDisks(hardDisks)
                    };
                var osDisk = GetOSHardDisk(root.Element(Namespace + "OSVirtualHardDisk"));
                persistentVirtualMachine.OSHardDisk = osDisk;
                // check and add linux as an OS
                if (osDisk.OS.ToLower() == "linux")
                {
                    persistentVirtualMachine.OperatingSystemConfigurationSet = new LinuxConfigurationSet() { HostName = GetStringValue(roleInstance.Element(Namespace + "HostName")) };
                }

                listVm.Add(persistentVirtualMachine);
            }

            return listVm;
        }

        #region Configuration Sets

        private NetworkConfigurationSet GetNetworkConfigurationSet(XElement configurationSet)
        {
            NetworkConfigurationSet networkConfigurationSet = null;
            if (configurationSet.Element(Namespace + "ConfigurationSetType").Value == "NetworkConfiguration")
            {
                networkConfigurationSet = new NetworkConfigurationSet();
                var subnets = configurationSet.Element(Namespace + "SubnetNames");
                if (subnets != null)
                {
                    var subnet = configurationSet.Element(Namespace + "SubnetName");
                    if (subnet != null)
                    {
                        networkConfigurationSet.SubnetName = (string) subnet;
                    }
                }
                networkConfigurationSet.InputEndpoints= new InputEndpoints();
                var endpoints = configurationSet.Descendants(Namespace + "InputEndpoint");
                foreach (var endpoint in endpoints)
                {
                    var inputEndpoint = new InputEndpoint
                    {
                        EndpointName = GetStringValue(endpoint.Element(Namespace + "Name")),
                        Port = GetIntValue(endpoint.Element(Namespace + "Port")),
                        LocalPort = GetIntValue(endpoint.Element(Namespace + "LocalPort")),
                        Protocol = GetEnumValue<Protocol>(endpoint.Element(Namespace + "Protocol")),
                        Vip = GetStringValue(endpoint.Element(Namespace + "Vip"))
                    };
                    networkConfigurationSet.InputEndpoints.AddEndpoint(inputEndpoint);
                }
            }
            return networkConfigurationSet;
        }

        private List<DataVirtualHardDisk> GetHardDisks(XElement disks)
        {
            var hardDiskElements = disks.Elements();
            return hardDiskElements.Select(hardDiskElement => new DataVirtualHardDisk()
            {
                DiskName = GetStringValue(hardDiskElement.Element(Namespace + "DiskName")),
                DiskLabel = GetStringValue(hardDiskElement.Element(Namespace + "DiskLabel")),
                HostCaching = GetEnumValue<HostCaching>(hardDiskElement.Element(Namespace + "HostCaching")),
                LogicalDiskSizeInGB = GetIntValue(hardDiskElement.Element(Namespace + "LogicalDiskSizeInGB")),
                LogicalUnitNumber = GetIntValue(hardDiskElement.Element(Namespace + "LogicalUnitNumber")),
                MediaLink = GetStringValue(hardDiskElement.Element(Namespace + "MediaLink"))
            }).ToList();
        }

        private OSVirtualHardDisk GetOSHardDisk(XElement disk)
        {
            return new OSVirtualHardDisk
            {
                DiskLabel = GetStringValue(disk.Element(Namespace + "DiskLabel")),
                DiskName = GetStringValue(disk.Element(Namespace + "DiskName")),
                HostCaching = GetEnumValue<HostCaching>(disk.Element(Namespace + "HostCaching")),
                MediaLink = GetStringValue(disk.Element(Namespace + "MediaLink")),
                SourceImageName = GetStringValue(disk.Element(Namespace + "SourceImageName")),
                OS = GetStringValue(disk.Element(Namespace + "OS"))
            };
        }

        #endregion

    }
}
