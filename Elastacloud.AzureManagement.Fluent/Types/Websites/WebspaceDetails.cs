/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The connection string info used for each connectionstring 
    /// </summary>
    public class WebspaceDetails
    {
        /// <summary>
        /// The size of the average number of instances
        /// </summary>
        public InstanceSize InstanceSize { get; set; }
        /// <summary>
        /// The name of the server farm - default is DefaultServerFarm
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The number of instances bound to the server farm
        /// </summary>
        public int InstanceCount { get; set; }
    }
}