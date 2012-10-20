/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.SqlAzure;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.SqlAzure.Classes;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.SqlAzure
{
    /// <summary>
    /// The Sql Azure Manager class will act to send the SQL Azure requests to the management service
    /// </summary>
    public class SqlAzureManager : IAzureManager, ISqlAzureServer, ISqlAzureDatabase, ISqlAzureSecurity,
                                   ISqlAzureDatabaseActivity, ISqlCertificateActivity, ISqlCompleteActivity
    {
        /// <summary>
        /// Dictionary of username and passwords for DbUsers
        /// </summary>
        internal readonly Dictionary<string, string> DbUsers = new Dictionary<string, string>();

        /// <summary>
        /// A list of firewall rules to apply to the Db
        /// </summary>
        internal readonly List<ISqlAzureFirewallRule> FirewallRules = new List<ISqlAzureFirewallRule>();

        /// <summary>
        /// A list of Sql Scripts to apply to the Db
        /// </summary>
        internal readonly List<string> SqlScripts = new List<string>();

        /// <summary>
        /// The command to create the Sql Azure instance
        /// </summary>
        internal AddNewSqlServerCommand AddNewServerCommand;

        /// <summary>
        /// The command to delete the Sql Azure server instance
        /// </summary>
        //internal DeleteSqlServerCommand DeleteSqlServerCommand;

        /// <summary>
        /// Construction of the Sql Azure Manager class on a particular subscription
        /// </summary>
        /// <param name="subscriptionId">The relevant subscription id</param>
        internal SqlAzureManager(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        #region Implementation of ISqlAzureServer

        /// <summary>
        /// Used to add a new Sql Azure Server instance
        /// </summary>
        /// <param name="location">the location of the new server</param>
        /// <returns>An ISqlCertificateActivity interface</returns>
        public ISqlCertificateActivity AddNewServer(string location)
        {
            SqlAzureLocation = location;
            ActionType = ActionType.Create;
            return this;
        }

        /// <summary>
        /// Deletes a Sql Azure server instance in the cloud
        /// </summary>
        /// <param name="name">The given name of the Sql Azure server</param>
        /// <returns>An ICertificateActivity interface</returns>
        public ISqlCertificateActivity DeleteServer(string name)
        {
            SqlAzureServerName = name;
            ActionType = ActionType.Delete;
            return this;
        }

        /// <summary>
        /// Given an existing server currently uses this to perform a database addition to 
        /// </summary>
        /// <param name="name">The name of the server</param>
        /// <returns>An ISqlCertificateActivity interface</returns>
        public ISqlCertificateActivity UseSqlAzureServerWithName(string name)
        {
            // TODO: check the server exists here
            SqlAzureServerName = name;
            ActionType = ActionType.Update;
            return this;
        }

        #endregion

        #region Implementation of ISqlAzureDatabase

        /// <summary>
        /// Adds a new database to a SQL Azure server instance 
        /// </summary>
        /// <param name="name">The name of the database</param>
        /// <returns>An ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity ISqlAzureDatabase.AddNewDatabase(string name)
        {
            SqlAzureDatabaseName = name;

            return this;
        }

        #endregion

        #region Implementation of ISqlAzureSecurity

        /// <summary>
        /// Adds credentials to a Sql Azure server instance
        /// </summary>
        /// <param name="username">The username to add</param>
        /// <param name="password">The password to add commensurate with the Sql Azure password rules</param>
        /// <returns>An ISqlAzureDatabase interface</returns>
        ISqlAzureDatabase ISqlAzureSecurity.WithSqlAzureCredentials(string username, string password)
        {
            SqlAzureUsername = username;
            SqlAzurePassword = password;

            return this;
        }

        /// <summary>
        /// Adds a new firewall rule to the Sql Azure server 
        /// </summary>
        /// <param name="rulename">The name of the rule - can be anything - word or phrase</param>
        /// <param name="iplow">The low IP value</param>
        /// <param name="iphigh">The high IP value</param>
        /// <returns>An ISqlAzureSecurity interface</returns>
        ISqlAzureSecurity ISqlAzureSecurity.AddNewFirewallRule(string rulename, string iplow, string iphigh)
        {
            FirewallRules.Add(new AddNewFirewallRuleCommand(rulename, iplow, iphigh)
                                  {
                                      Certificate = ManagementCertificate,
                                      SubscriptionId = SubscriptionId
                                  });
            return this;
        }

        /// <summary>
        /// A firewall rule for the requesting IP address
        /// </summary>
        /// <param name="rulename">the rulename for the firewall rule</param>
        /// <returns>The ISqlAzureSecurity interface</returns>
        ISqlAzureSecurity ISqlAzureSecurity.AddNewFirewallRuleWithMyIp(string rulename)
        {
            FirewallRules.Add(new AddNewFirewallRuleWithIpDetectCommand(rulename)
                                  {
                                      Certificate = ManagementCertificate,
                                      SubscriptionId = SubscriptionId
                                  });
            return this;
        }

        /// <summary>
        /// Adds a hosted service IP rule of 0.0.0.0
        /// </summary>
        /// <returns>An ISqlAzureSecurity interface</returns>
        ISqlAzureSecurity ISqlAzureSecurity.AddNewFirewallRuleForWindowsAzureHostedService()
        {
            FirewallRules.Add(new AddNewFirewallRuleCommand("Azure Hosted Services", "0.0.0.0", "0.0.0.0")
                                  {
                                      Certificate = ManagementCertificate,
                                      SubscriptionId = SubscriptionId
                                  });
            return this;
        }

        /// <summary>
        /// Adds no database activity to the script mix
        /// </summary>
        /// <returns>An ISqlCompleteActivity interface</returns>
        ISqlCompleteActivity ISqlAzureSecurity.WithNoDatabaseActivity()
        {
            // do nothing here just as a passthru
            return this;
        }

        #endregion

        #region Implementation of IAzureManager

        public event EventReached AzureTaskComplete;
        public string SubscriptionId { get; set; }
        public X509Certificate2 ManagementCertificate { get; set; }

        #endregion

        #region Implementation of ISqlCertificateActivity

        ISqlAzureSecurity ISqlCertificateActivity.AddCertificate(X509Certificate2 certificate)
        {
            ManagementCertificate = certificate;
            return this;
        }

        ISqlAzureSecurity ISqlCertificateActivity.AddPublishSettings(string path)
        {
            var settings = new PublishSettingsExtractor(path);
            ManagementCertificate = settings.GetCertificateFromFile();
            return this;
        }

        ISqlAzureSecurity ISqlCertificateActivity.AddCertificateFromStore(string thumbprint)
        {
            ManagementCertificate = PublishSettingsExtractor.FromStore(thumbprint);
            return this;
        }

        #endregion

        #region Implementation of ISqlAzureDatabaseActivity

        /// <summary>
        /// Used to read in a set of scripts given a scripts directory
        /// </summary>
        /// <param name="scriptDirectory">The physical file path of the scripts directory</param>
        /// <returns>An ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity ISqlAzureDatabaseActivity.ExecuteScripts(string scriptDirectory)
        {
            var info = new DirectoryInfo(scriptDirectory);
            FileInfo[] sqlScriptFiles = info.GetFiles("*.sql");
            foreach (FileInfo fi in sqlScriptFiles)
            {
                var fileInfo = new FileInfo(fi.FullName);
                SqlScripts.Add(fileInfo.OpenText().ReadToEnd());
            }
            return this;
        }

        /// <summary>
        /// Add a new database administrator to the server
        /// </summary>
        /// <param name="username">the username to use with the server</param>
        /// <param name="password">the password to use in line with the applied password rules</param>
        /// <returns>returns an ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity ISqlAzureDatabaseActivity.AddNewDatabaseAdminUser(string username, string password)
        {
            DbUsers.Add(username, password);
            return this;
        }

        /// <summary>
        /// Executes an arbitrary piece of Sql 
        /// </summary>
        /// <param name="sql">A sql statement to execute</param>
        /// <returns>An ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity ISqlAzureDatabaseActivity.ExecuteSql(string sql)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to return an IServiceTransaction interface
        /// </summary>
        IServiceTransaction ISqlCompleteActivity.Go()
        {
            return new SqlAzureTransaction(this);
        }

        #endregion

        /// <summary>
        /// The type of Action which will be used with the specific service type
        /// </summary>
        internal ActionType ActionType { get; set; }

        /// <summary>
        /// The Sql Azure server name
        /// </summary>
        public string SqlAzureServerName { get; set; }

        /// <summary>
        /// The Sql Azure database name 
        /// </summary>
        public string SqlAzureDatabaseName { get; set; }

        /// <summary>
        /// The Sql Azure username for the string 
        /// </summary>
        public string SqlAzureUsername { get; set; }

        /// <summary>
        /// The Sql Azure password for the string 
        /// </summary>
        public string SqlAzurePassword { get; set; }

        /// <summary>
        /// The Sql Azure location where this will be created
        /// </summary> 
        public string SqlAzureLocation { get; set; }

        /// <summary>
        /// Creates and logs a messages
        /// </summary>
        /// <param name="point">The different event points that are reached</param>
        /// <param name="message">The string message from the exception</param>
        internal void WriteComplete(EventPoint point, string message)
        {
            if (AzureTaskComplete != null)
                AzureTaskComplete(point, message);
        }

        #region Implementation of ISqlCompleteActivity

        /// <summary>
        /// Executes the command to build the server and database
        /// </summary>
        /// <returns>An IServerTransaction interface</returns>
        public IServiceTransaction Go()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}