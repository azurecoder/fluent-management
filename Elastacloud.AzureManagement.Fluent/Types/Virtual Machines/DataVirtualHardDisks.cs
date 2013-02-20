/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// The container for the OS and Data disks - acts as a collection
    /// </summary>
    public class DataVirtualHardDisks : ICustomXmlSerializer
    {
        /// <summary>
        /// Creates a list of hard drives which can be used for the OS or data
        /// </summary>
        public DataVirtualHardDisks()
        {
            HardDiskCollection = new List<DataVirtualHardDisk>();
        }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "DataVirtualHardDisks");
            HardDiskCollection.ForEach(a => element.Add(a.GetXmlTree()));
            return element;
        }

        #endregion

        /// <summary>
        /// Gets or sets the collection of virtual hard disks
        /// </summary>
        public List<DataVirtualHardDisk> HardDiskCollection { get; set; }
    }
}