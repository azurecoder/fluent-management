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
    }
}
