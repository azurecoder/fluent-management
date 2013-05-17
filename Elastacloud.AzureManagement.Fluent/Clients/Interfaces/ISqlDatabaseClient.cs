using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    /// <summary>
    /// A client interface to allow connectivity to WASD
    /// </summary>
    public interface ISqlDatabaseClient
    {
        /// <summary>
        /// Adds IP addresses to the WASD firewall for a 
        /// </summary>
        void AddIpsToSqlFirewallFromCloudService(string cloudServiceName, bool removeAllOtherRules = true, DeploymentSlot slot = DeploymentSlot.Production);
        /// <summary>
        /// The name of the WASD server
        /// </summary>
        string ServerName { get; set; }
        /// <summary>
        /// Gets a count of the number of databases that exist on the server
        /// </summary>
        int DatabaseCount { get; }
        /// <summary>
        /// Deletes a database and also deletes the server if the database is the last one 
        /// </summary>
        void DeleteDatabase(string name, bool deleteServerIfLastDatabase = true);
        /// <summary>
        /// The login associated with the WASS
        /// </summary>
        string AdministratorServerLogin { get; set; }
        /// <summary>
        /// The passwords associated with the WASS
        /// </summary>
        string AdministratorServerPassword { get; set; }
    }
}
