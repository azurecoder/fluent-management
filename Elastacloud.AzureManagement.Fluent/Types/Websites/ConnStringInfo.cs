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
    public class ConnStringInfo
    {
        /// <summary>
        /// The name of the connection string
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The actual connection string
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// The type of connection string - e.g. SQLAzure
        /// </summary>
        public string Type { get; set; }
    }
}