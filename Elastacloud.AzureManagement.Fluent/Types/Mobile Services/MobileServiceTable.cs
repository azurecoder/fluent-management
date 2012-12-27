/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
namespace Elastacloud.AzureManagement.Fluent.Types.MobileServices
{
    /// <summary>
    /// Defines a mobile services table
    /// </summary>
    public class MobileServiceTable
    {
        /// <summary>
        /// The name of the table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Describes the permission for the insert script
        /// </summary>
        public Roles InsertPermission { get; set; }
        /// <summary>
        /// Describes the permission for the update script
        /// </summary>
        public Roles UpdatePermission { get; set; }
        /// <summary>
        /// Describes the permission for the read script
        /// </summary>
        public Roles ReadPermission { get; set; }
        /// <summary>
        /// Describes the permission for the delete script
        /// </summary>
        public Roles DeletePermission { get; set; }
        /// <summary>
        /// The size of the table in bytes
        /// </summary>
        public int SizeInBytes { get; set; }
        /// <summary>
        /// The number of indexes on the table
        /// </summary>
        public int NumberOfIndexes { get; set; }
        /// <summary>
        /// the number of records the table contains
        /// </summary>
        public int NumberOfRecords { get; set; }
    }
}