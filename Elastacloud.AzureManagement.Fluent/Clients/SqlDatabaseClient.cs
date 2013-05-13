using System;
using System.Collections.Generic;
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
    /// Used to connect to the Github repos
    /// </summary>
    public class SqlDatabaseClient : ISqlDatabaseClient
    {
        private readonly string _subscriptionId;
        private readonly X509Certificate2 _managementCertificate;

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

        #endregion
    }
}
