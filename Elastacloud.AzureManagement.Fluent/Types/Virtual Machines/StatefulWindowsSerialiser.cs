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

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Used to serialise all XMl windows information whcih comes back from the command layer
    /// </summary>
    public class StatefulWindowsSerialiser : StatefulSerialiser
    {
        private readonly XDocument _document;
        /// <summary>
        /// Used to construct the stateful serialiser
        /// </summary>
        public StatefulWindowsSerialiser(XDocument doc)
        {
            _document = doc;
        }

        /// <summary>
        /// Gets a VM role for Windows which 
        /// </summary>
        public override PersistentVMRole GetVmRole()
        {
            var persistentVirtualMachine = new PersistentVMRole();
            // get all of the root properties
            var firstRoot = _document.Descendants(Namespace + "RoleList").FirstOrDefault();
            var root = firstRoot.Element(Namespace + "Role");
            if(root.Attribute(TypeNamespace + "type").Value != "PersistentVMRole")
                throw new ApplicationException("non persistent vm role detected");

            persistentVirtualMachine.AvailabilityNameSet = GetStringValue(root.Element(Namespace + "AvailabilitySetName"));
            persistentVirtualMachine.RoleSize = GetEnumValue<VmSize>(root.Element(Namespace + "RoleSize"));
            persistentVirtualMachine.RoleName = GetStringValue(root.Element(Namespace + "RoleName"));
            //persistentVirtualMachine.IPAddress = GetStringValue(root.Element(Namespace + "IpAddress"));
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
            return persistentVirtualMachine;
        }

        #region Configuration Sets

        private NetworkConfigurationSet GetNetworkConfigurationSet(XElement configurationSet)
        {
            NetworkConfigurationSet networkConfigurationSet = null;
            if (configurationSet.Element(Namespace + "ConfigurationSetType").Value == "NetworkConfiguration")
            {
                networkConfigurationSet = new NetworkConfigurationSet();
                networkConfigurationSet.InputEndpoints= new InputEndpoints();
                var endpoints = configurationSet.Descendants(Namespace + "InputEndpoints");
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
                SourceImageName = GetStringValue(disk.Element(Namespace + "SourceImageName"))
            };
        }

        #endregion
    }
}
