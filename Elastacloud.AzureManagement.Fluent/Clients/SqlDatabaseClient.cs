using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.SqlAzure;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Newtonsoft.Json.Linq;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Constructs a SqlDatabase client used to manipulate WASD
    /// </summary>
    public class SqlDatabaseClient : ISqlDatabaseClient
    {
        private readonly string _subscriptionId;
        private readonly X509Certificate2 _managementCertificate;

        /// <summary>
        /// Constructs a SqlDatabase client used to manipulate WASD
        /// </summary>
        public SqlDatabaseClient(string subscriptionId, X509Certificate2 managementCertificate, string serverName)
        {
            ServerName = serverName;
            _managementCertificate = managementCertificate;
            _subscriptionId = subscriptionId;
        }

        #region Implementation of ISqlDatabaseClient

        /// <summary>
        /// Adds IP addresses to the WASD firewall for a 
        /// </summary>
        public void AddIpsToSqlFirewallFromCloudService(string cloudServiceName, bool removeAllOtherRules = true, DeploymentSlot slot = DeploymentSlot.Production)
        {
            if(ServerName == null)
                throw new FluentManagementException("unable to continue without windows azure sql database server name", "SqlDatabaseClient");

            if (removeAllOtherRules)
            {
                // get these rules 
                var command = new ListFirewallRulesCommand(ServerName)
                    {
                        SubscriptionId = _subscriptionId,
                        Certificate = _managementCertificate
                    };
                command.Execute();
                foreach (var rule in command.FirewallRules)
                {
                    var ruleCommand = new DeleteSqlFirewallRuleCommand(ServerName, rule.RuleName)
                        {
                            SubscriptionId = _subscriptionId,
                            Certificate = _managementCertificate
                        };
                    ruleCommand.Execute();
                }
            }

            var inputs = new LinqToAzureInputs()
            {
                ManagementCertificateThumbprint = _managementCertificate.Thumbprint,
                SubscriptionId = _subscriptionId
            };
            // build up a filtered query to check the new account
            var cloudServiceQueryable = new LinqToAzureOrderedQueryable<CloudService>(inputs);
            // get only production deployments
            var query = from service in cloudServiceQueryable
                        where service.Deployments.Count != 0
                        && service.Deployments.Any(a => a.Slot == slot)
                        select service;
            var cloudService = query.First();
            // enumerate the cloud service deployment and add the ips to the database firewall
            var addRuleCommand = new AddNewFirewallRuleCommand(cloudServiceName,
                                                               cloudService.Deployments.First().RoleInstances.First().VirtualIpAddress,
                                                               cloudService.Deployments.First().RoleInstances.First().VirtualIpAddress)
                {
                    SubscriptionId = _subscriptionId,
                    Certificate = _managementCertificate
                };
            addRuleCommand.ConfigureFirewallCommand(ServerName);
            addRuleCommand.Execute();
        }

        /// <summary>
        /// The name of the WASD server
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets a count of the number of databases that exist on the server
        /// </summary>
        public int DatabaseCount
        {
            get
            {
                var connection = GetConnection("master");
                return ExecuteCountCommand(connection, "SELECT COUNT(*) FROM SYS.DATABASES");
            }
        }

        /// <summary>
        /// Deletes a database and also deletes the server if the database is the last one 
        /// </summary>
        public void DeleteDatabase(string name, bool deleteServerIfLastDatabase = true)
        {
            CheckLoginCredentials();
            // we need to add IP detect and add to the firewall first
            var firewallCommandWithIpDetect = new AddNewFirewallRuleWithIpDetectCommand("mobileser")
                {
                    SubscriptionId = _subscriptionId,
                    Certificate = _managementCertificate
                };
            firewallCommandWithIpDetect.ConfigureFirewallCommand(ServerName);
            firewallCommandWithIpDetect.Execute();
            // get the connection to the server
            var connection = GetConnection("master");
            // drop the named database
            ExecuteCommand(connection, "DROP DATABASE " + name);
            // Gets the count of databases left on the server
            if (deleteServerIfLastDatabase && DatabaseCount == 1)
            {
                // delete the server
                var command = new DeleteSqlServerCommand(ServerName)
                    {
                        SubscriptionId = _subscriptionId,
                        Certificate = _managementCertificate
                    };
                command.Execute();
            }
        }

        /// <summary>
        /// The login associated with the WASS
        /// </summary>
        public string AdministratorServerLogin { get; set; }

        /// <summary>
        /// The passwords associated with the WASS
        /// </summary>
        public string AdministratorServerPassword { get; set; }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets a connection to a Sql Database
        /// </summary>
        private SqlConnection GetConnection(string dbName)
        {
            string connectionString =
                String.Format(
                    "server=tcp:{0}.database.windows.net; database={1}; user id={2}@{0}; password={3}; Trusted_Connection=False; Encrypt=True;",
                    ServerName, dbName, AdministratorServerLogin, AdministratorServerPassword);
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        /// <summary>
        /// Checks that the login credentials for the server have been set
        /// </summary>
        private void CheckLoginCredentials()
        {
            if (String.IsNullOrEmpty(AdministratorServerLogin) || String.IsNullOrEmpty(AdministratorServerPassword))
            {
                throw new FluentManagementException("unable to continue login and password not specified", "SqlDatabaseClient");
            }
        }

        /// <summary>
        /// Executes a set of commands against a Sql database
        /// </summary>
        private void ExecuteCommand(SqlConnection connection, string sql)
        {
            var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Executes a count command against a table
        /// </summary>
        private int ExecuteCountCommand(SqlConnection connection, string sql)
        {
            var command = new SqlCommand(sql, connection);
           return (int) command.ExecuteScalar();
        }

        #endregion
    }
}
