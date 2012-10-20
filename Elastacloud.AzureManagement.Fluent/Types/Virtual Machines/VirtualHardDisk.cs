/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// The base class used to express a virtual hard disk for an iaas deployment
    /// </summary>
    public abstract class VirtualHardDisk : ICustomXmlSerializer
    {
        /// <summary>
        /// Whether the disk is readonly or readwrite
        /// </summary>
        public HostCaching HostCaching { get; set; }

        /// <summary>
        /// The disk label (volume)
        /// </summary>
        public string DiskLabel { get; set; }

        /// <summary>
        /// The name of the disk
        /// </summary>
        public string DiskName { get; set; }

        /// <summary>
        /// The blob url of the media link which contains the disk data 
        /// </summary>
        public string MediaLink { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public abstract XElement GetXmlTree();

        #endregion
    }
}