/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/


using System;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Contains the detailed information for image property generation 
    /// </summary>
    public class ImageProperties
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string MediaLink { get; set; }
        public PlatformType OperatingSystem { get; set; }
        public string Eula { get; set; }
        public string Description { get; set; }
        public string ImageFamily { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool? IsPremium { get; set; }
        public bool? ShowInGui { get; set; }
    }
}
