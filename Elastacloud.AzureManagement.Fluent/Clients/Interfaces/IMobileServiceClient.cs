/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    public interface IMobileServiceClient
    {
        /// <summary>
        /// Used to create a new mobile service
        /// </summary>
        /// <param name="serviceName">The name of the mobile service</param>
        /// <param name="sqlUsername">the name of the sql user</param>
        /// <param name="sqlPassword">The sql password</param>
        void CreateMobileServiceWithNewDb(string serviceName, string sqlUsername, string sqlPassword);

        /// <summary>
        /// Adds a table to mobile services
        /// </summary>
        /// <param name="tableName">the name of the table</param>
        /// <param name="defaultPermission">Sets the default permission for the table scripts</param>
        void AddTable(string tableName, Types.MobileServices.Roles defaultPermission);

        /// <summary>
        /// Adds a script for a crud operation to the table
        /// </summary>
        /// <param name="operationType">The type of operation</param>
        /// <param name="tableName">The name of the WAMS table</param>
        /// <param name="script">The script to upload</param>
        /// <param name="permission">What the permissions of the script are</param>
        void AddTableScript(CrudOperation operationType, string tableName, string script, Types.MobileServices.Roles permission);
        /// <summary>
        /// Adds a scheduled job to WAMS
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <param name="script">The actual script</param>
        /// <param name="intervalInMinutes">The interval in minutes</param>
        void AddSchedulerScript(string name, string script, int intervalInMinutes);
        /// <summary>
        /// Updates any of the settable prooperties of the mobile service
        /// </summary>
        void Update();
        /// <summary>
        /// Refreshes the details of the mobile service
        /// </summary>
        void Refresh();
        /// <summary>
        /// Regenerates both the master and the application keys
        /// </summary>
        void RegenerateKeys();
        /// <summary>
        /// The mobile service account key
        /// </summary>
        string ApplicationKey { get; }
        /// <summary>
        /// Used to restart the mobile service
        /// </summary>
        void Restart();
        /// <summary>
        /// The secret used to access the mobile service
        /// </summary>
        string MasterKey { get; }
        /// <summary>
        /// The url of the mobile service
        /// </summary>
        string ApplicationUrl { get; }
        /// <summary>
        /// Used to get or set a description of the mobile service
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// The location of the mobile service
        /// </summary>
        string Location { get; set; }
        /// <summary>
        /// The name of the Sql Azure server
        /// </summary>
        string SqlAzureServerName { get; }
        /// <summary>
        /// The name of the Sql Azure database
        /// </summary>
        string SqlAzureDbName { get; }
        /// <summary>
        /// The username that is used to access the sql azure db
        /// </summary>
        string SqlAzureUsername { get; set; }
        /// <summary>
        /// The password that is used to access the Sqlazure db
        /// </summary>
        string SqlAzurePassword { get; set; }
        /// <summary>
        /// The name of the mobile service sql name
        /// string refName = "ZumoSqlServer_" + Guid.NewGuid().ToString("n");
        /// </summary>
        string MobileServiceSqlName { get; }
        /// <summary>
        /// The name of the mobile sevrice Db name
        /// string refName = "ZumoSqlDatabase_" + Guid.NewGuid().ToString("n");
        /// </summary>
        string MobileServiceDbName { get; }
        /// <summary>
        /// The name of the mobile service in context
        /// </summary>
        string MobileServiceName { get; }
        /// <summary>
        /// Gets the state of the mobile service
        /// </summary>
        State MobileServiceState { get; }
        /// <summary>
        /// Gets a list of mobile services tables
        /// </summary>
        List<MobileServiceTable> Tables { get; }
        /// <summary>
        /// Gets or sets whether the dynamic schema is enabled or not
        /// </summary>
        bool DynamicSchemaEnabled { get; set; }
        /// <summary>
        /// The client secret for live id access
        /// </summary>
        string MicrosoftAccountClientSecret { get; set; }
        /// <summary>
        /// The client id for live id access
        /// </summary>
        string MicrosoftAccountClientId { get; set; }
        /// <summary>
        /// The package sid for wns 
        /// </summary>
        string MicrosoftAccountPackageSID { get; set; }
        /// <summary>
        /// The client id for facebook access
        /// </summary>
        string FacebookClientId { get; set; }
        /// <summary>
        /// The client secret for facebook access
        /// </summary>
        string FacebookClientSecret { get; set; }
        /// <summary>
        /// The client id for google access
        /// </summary>
        string GoogleClientId { get; set; }
        /// <summary>
        /// The client secret for google access
        /// </summary>
        string GoogleClientSecret { get; set; }
        /// <summary>
        /// The client id for twitter access
        /// </summary>
        string TwitterClientId { get; set; }
        /// <summary>
        /// The client secret for twitter access
        /// </summary>
        string TwitterClientSecret { get; set; }
        /// <summary>
        /// The mobile service log entry entries
        /// </summary>
        List<MobileServiceLogEntry> Logs { get; }
        /// <summary>
        /// Deletes a mobile service along with any linked database server
        /// </summary>
        void Delete(bool deleteSqlAzureDatabase = true);
        /// <summary>
        /// Gets or sets the total number of instances for all of the mobile services
        /// </summary>
        int TotalInstanceCount { get; set; }
        /// <summary>
        /// Whether this is free or reserved
        /// </summary>
        ComputeMode ComputeMode { get; set; }
    }
}