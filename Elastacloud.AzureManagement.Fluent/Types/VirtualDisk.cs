/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/


namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// The name of the virtual disk which will be brought back 
    /// </summary>
    public class VirtualDisk
    {
        /// <summary>
        /// The affinity group the VD belongs in
        /// </summary>
        public string AffinityGroup { get; set; }
        /// <summary>
        /// The OS Linux or Windows
        /// </summary>
        public string OS { get; set; }
        /// <summary>
        /// The location of the disk e.g. North Europe
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// The logical size of the disk
        /// </summary>
        public int LogicalSizeInGB { get; set; }
        /// <summary>
        /// The medialink of the disk
        /// </summary>
        public string MediaLink { get; set; }
        /// <summary>
        /// The name of the disk 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// If this is a disk template then reference this here
        /// </summary>
        public string SourceImageName { get; set; }
        /// <summary>
        /// The VM that this disk is attached to
        /// </summary>
        public AttachedVM VM { get; set; }
    }
}
