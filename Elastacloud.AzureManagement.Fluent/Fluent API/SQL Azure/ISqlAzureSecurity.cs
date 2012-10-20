/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.SqlAzure
{
    /// <summary>
    /// An interface that adds different facets of security to the SQL Azure Server instance
    /// </summary>
    public interface ISqlAzureSecurity
    {
        /// <summary>
        /// Used to add the SQL Azure credentials 
        /// </summary>
        ISqlAzureDatabase WithSqlAzureCredentials(string username, string password);

        /// <summary>
        /// Adds a new firewall rule with a low IP and high IP address
        /// </summary>
        ISqlAzureSecurity AddNewFirewallRule(string rulename, string iplow, string iphigh);

        /// <summary>
        /// Adds a firewall rule with the autodetect flag
        /// </summary>
        ISqlAzureSecurity AddNewFirewallRuleWithMyIp(string rulename);

        /// <summary>
        /// Adds a firewall rule with the 0.0.0.0 IP range used for hosted service access
        /// </summary>
        ISqlAzureSecurity AddNewFirewallRuleForWindowsAzureHostedService();

        /// <summary>
        /// Adds no database activity to the script mix
        /// </summary>
        /// <returns>An ISqlCompleteActivity interface</returns>
        ISqlCompleteActivity WithNoDatabaseActivity();
    }
}