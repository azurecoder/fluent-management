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
using System.Reflection;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Class used to configure an OS virtual hard disk for the virtual machine - this drive is always c
    /// </summary>
    public class OSVirtualHardDisk : VirtualHardDisk, ICustomXmlSerializer
    {
        /// <summary>
        /// The name of the source image used to build the operating system 
        /// </summary>
        public string SourceImageName { get; set; }
        /// <summary>
        /// This contains the gallery of vm templates
        /// </summary>
        public static Dictionary<string, string> VmTemplates = new Dictionary<string, string>();
        /// <summary>
        /// used to generate random names for the os disks 
        /// </summary>
        private static readonly RandomAccountName Namer = new RandomAccountName();
        /// <summary>
        /// The operating system which is being tested for
        /// </summary>
        public string OS { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public override XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "OSVirtualHardDisk",
//                                       new XElement(Namespaces.NsWindowsAzure + "HostCaching", HostCaching.ToString()),
                                       new XElement(Namespaces.NsWindowsAzure + "MediaLink", MediaLink),
                                       new XElement(Namespaces.NsWindowsAzure + "SourceImageName", SourceImageName)
                );
            if (DiskLabel != null)
                element.Add(new XElement(Namespaces.NsWindowsAzure + "DiskLabel", DiskLabel));
            if (DiskName != null)
                element.Add(new XElement(Namespaces.NsWindowsAzure + "DiskName", DiskName));
            return element;
        }

        #endregion

        #region Template

        /// <summary>
        /// This gets the host OS image of Windows Server Data Centre and SQL Server 2012
        /// </summary>
        /// <param name="properties">The path to the media space in blob storage where the host vhd will be placed</param>
        /// <returns>An OSVirtualHardDisk instance</returns>
        public static OSVirtualHardDisk GetOSImageFromTemplate(VirtualMachineProperties properties)
        {
            /*<OSVirtualHardDisk>
                        <MediaLink>http://elastacacheweb.blob.core.windows.net/vhds/elastasql.vhd</MediaLink>
                        <SourceImageName>MSFT__Sql-Server-11EVAL-11.0.2215.0-05152012-en-us-30GB.vhd</SourceImageName>
              </OSVirtualHardDisk>*/
            string templateDetails = null;

            string vmType = properties.VirtualMachineType.ToString();
            if (String.IsNullOrEmpty(properties.CustomTemplateName))
            {
                AddVmTemplatesToDictionary();
                if (VmTemplates.ContainsKey(vmType))
                {
                    templateDetails = VmTemplates[vmType];
                }
            }

            if(templateDetails == null && properties.CustomTemplateName == null)
                throw new FluentManagementException("no template specified cannot proceed", "GetOSImageFromTemplate");

            if (properties.CustomTemplateName != null)
                templateDetails = properties.CustomTemplateName;

            return new OSVirtualHardDisk
            {
                DiskLabel = "OsDisk",
                DiskName = "OsDisk",
                MediaLink = String.Format("http://{0}.blob.core.windows.net/vhds/{1}{2}.vhd", properties.StorageAccountName, Namer.GetNameFromInitString("os"), DateTime.Now.ToString("ddMMyyhhmmss-ffffff")),

                SourceImageName = templateDetails,
                HostCaching = HostCaching.ReadWrite,
            };
        }

        #endregion

        private static void AddVmTemplatesToDictionary()
        {
            // get the constants if they haven't been got already
            if (VmTemplates.Count > 0)
                return;
            // if not then add them to the collection
            var constants = new VmConstants();
            FieldInfo[] thisObjectProperties = constants.GetType().GetFields();
            foreach (FieldInfo info in thisObjectProperties)
            {
                if (info.IsLiteral && info.IsStatic)
                {
                    VmTemplates.Add(info.Name, (string)info.GetRawConstantValue());
                }
            }
        }
    }
}