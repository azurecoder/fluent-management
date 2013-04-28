/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
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
        /// <param name="storageAccountName">The path to the media space in blob storage where the host vhd will be placed</param>
        /// <param name="diskName">The name of the C: drive </param>
        /// <param name="diskLabel">The drive volume label for C:</param>
        /// <returns>An OSVirtualHardDisk instance</returns>
        public static OSVirtualHardDisk GetSqlServerOSImage(string storageAccountName, string diskName = null, string diskLabel = null)
        {
            /*<OSVirtualHardDisk>
                        <MediaLink>http://elastacacheweb.blob.core.windows.net/vhds/elastasql.vhd</MediaLink>
                        <SourceImageName>MSFT__Sql-Server-11EVAL-11.0.2215.0-05152012-en-us-30GB.vhd</SourceImageName>
                    </OSVirtualHardDisk>*/
            var namer = new RandomAccountName();
            return new OSVirtualHardDisk
                       {
                           DiskLabel = diskLabel,
                           DiskName = diskName,
                           MediaLink = String.Format("http://{0}.blob.core.windows.net/vhds/{1}{2}.vhd", storageAccountName, namer.GetNameFromInitString("os"), DateTime.Now.ToString("ddmmyy")),
                           SourceImageName = "MSFT__Sql-Server-11EVAL-11.0.2215.0-05152012-en-us-30GB.vhd",
                           HostCaching = HostCaching.ReadWrite,
                       };
        }

        /// <summary>
        /// This gets the host OS image of Windows Server Data Centre and SQL Server 2012
        /// </summary>
        /// <param name="properties">The path to the media space in blob storage where the host vhd will be placed</param>
        /// <returns>An OSVirtualHardDisk instance</returns>
        public static OSVirtualHardDisk GetWindowsOSImageFromTemplate(WindowsVirtualMachineProperties properties)
        {
            /*<OSVirtualHardDisk>
                        <MediaLink>http://elastacacheweb.blob.core.windows.net/vhds/elastasql.vhd</MediaLink>
                        <SourceImageName>MSFT__Sql-Server-11EVAL-11.0.2215.0-05152012-en-us-30GB.vhd</SourceImageName>
              </OSVirtualHardDisk>*/
            string templateDetails = null;
            switch (properties.VirtualMachineType)
            {
                case VirtualMachineTemplates.BiztalkServer2013Enterprise:
                    templateDetails = VmConstants.VmTemplateBizTalkServer2013Enterprise;
                    break;
                case VirtualMachineTemplates.BiztalkServer2013Standard:
                    templateDetails = VmConstants.VmTemplateBizTalkServer2013Standard;
                    break;
                case VirtualMachineTemplates.SqlServer2012Enterprise:
                    templateDetails = VmConstants.VmTemplateSqlServer2012Enterprise;
                    break;
                case VirtualMachineTemplates.SqlServer2012Standard:
                    templateDetails = VmConstants.VmTemplateSqlServer2012Standard;
                    break;
                case VirtualMachineTemplates.SqlServer2012Web:
                    templateDetails = VmConstants.VmTemplateSqlServer2012Web;
                    break;
                case VirtualMachineTemplates.WindowsServer2008R2SP1_127GB:
                    templateDetails = VmConstants.VmTemplateWin2K8SP1_DataCentre_127GB;
                    break;
                case VirtualMachineTemplates.WindowsServer2008R2SP1_30GB:
                    templateDetails = VmConstants.VmTemplateWin2K8SP1_DataCentre_30GB;
                    break;
                case VirtualMachineTemplates.WindowsServer2012_127GB:
                    templateDetails = VmConstants.VmTemplateWin2012_DataCentre_127GB;
                    break;
                case VirtualMachineTemplates.WindowsServer2012_30GB:
                    templateDetails = VmConstants.VmTemplateWin2012_DataCentre_30GB;
                    break;
            }
            if(templateDetails == null && properties.CustomTemplateName == null)
                throw new FluentManagementException("no template specified cannot proceed", "CreateWindowsVirtualMachineDeploymentCommand");

            if (properties.CustomTemplateName != null)
                templateDetails = properties.CustomTemplateName;

            var namer = new RandomAccountName();
            return new OSVirtualHardDisk
            {
                DiskLabel = "OsDisk",
                DiskName = "OsDisk",
                MediaLink = String.Format("http://{0}.blob.core.windows.net/vhds/{1}{2}.vhd", properties.StorageAccountName, namer.GetNameFromInitString("os"), DateTime.Now.ToString("ddmmyy")),

                SourceImageName = templateDetails,
                HostCaching = HostCaching.ReadWrite,
            };
        }

        #endregion
    }
}