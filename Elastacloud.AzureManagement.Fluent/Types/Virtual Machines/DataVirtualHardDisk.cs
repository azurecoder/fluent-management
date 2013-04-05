/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Globalization;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Used to build a data virtual hard disk for the persistent VM - there can be many of these each 1TB in size
    /// </summary>
    public class DataVirtualHardDisk : VirtualHardDisk, ICustomXmlSerializer
    {
        /// <summary>
        /// This is the logical unit number of the drive which is an integer between 0 and 15 drive letters start from e:, d is volatile and c is the OS drive
        /// </summary>
        public int LogicalUnitNumber { get; set; }

        /// <summary>
        /// The logical disk size of the persistent drive which resides in Blob storage can be up to 1000
        /// </summary>
        public int LogicalDiskSizeInGB { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public override XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "DataVirtualHardDisk",
                                       new XElement(Namespaces.NsWindowsAzure + "LogicalUnitNumber",
                                                    LogicalUnitNumber.ToString(CultureInfo.InvariantCulture)),
                                       new XElement(Namespaces.NsWindowsAzure + "LogicalDiskSizeInGB",
                                                    LogicalDiskSizeInGB.ToString(CultureInfo.InvariantCulture)),
                                       new XElement(Namespaces.NsWindowsAzure + "MediaLink", MediaLink),
                                       new XElement(Namespaces.NsWindowsAzure + "HostCaching", HostCaching.ToString()));
            if (DiskLabel != null)
                element.Add(new XElement(Namespaces.NsWindowsAzure + "DiskLabel", DiskLabel));
            if (DiskName != null)
                element.Add(new XElement(Namespaces.NsWindowsAzure + "DiskName", DiskName));
            return element;
        }

        #endregion

        /// <summary>
        /// Gets a virtual disk located in blob storage and sets a default size to the disk of 30 GB and a logical unit no. of 0 (drive e:)
        /// </summary>
        /// <param name="storageAccountName">The path to blob storage for the disk</param>
        /// <param name="size">the size of the disk in GB - can be up to 1TB</param>
        /// <param name="logicalUnitNumber">number between 0 and 15 which is the logical unit of the drive</param>
        /// <param name="diskName">the name of the disk</param>
        /// <param name="diskLabel">the label of the disk</param>
        /// <returns></returns>
        public static DataVirtualHardDisk GetDefaultDataDisk(string storageAccountName, int size = 30, int logicalUnitNumber = 0, string diskName = null, string diskLabel = null)
        {
            var namer = new RandomAccountName();
            return new DataVirtualHardDisk
                       {
                           LogicalDiskSizeInGB = size,
                           LogicalUnitNumber = logicalUnitNumber,
                           HostCaching = HostCaching.None,
                           DiskLabel = diskLabel,
                           DiskName = diskName,
                           MediaLink =
                               String.Format("http://{0}.blob.core.windows.net/vhds/{1}-{2}.vhd", storageAccountName,
                                             namer.GetNameFromInitString(diskName), DateTime.Now.ToString("ddmmyy")),
                       };
        }
    }
}