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
using System.Data;
using System.Data.SqlClient;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Commands.SqlAzure;
using Elastacloud.AzureManagement.Fluent.Types;


namespace Elastacloud.AzureManagement.Fluent.SqlAzure.Classes
{
    /// <summary>
    /// Able to perpetuate a transaction for a SqlAzure database - only works one way
    /// </summary>
    public class SqlAzureTransaction : IServiceTransaction
    {
        /// <summary>
        /// The Sql Azure Manager class instance
        /// </summary>
        private readonly SqlAzureManager _manager;

        /// <summary>
        /// Contains the flags to denote whether the transaction has started and been successful
        /// </summary>
        private bool _started, _success;

        public SqlAzureTransaction(SqlAzureManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Creates an executes an Add new Sql Azure Server instance command
        /// </summary>
        /// <returns>A string name for the newly created server instance</returns>
        internal string AddNewSqlServer()
        {
            _manager.AddNewServerCommand = new AddNewSqlServerCommand(_manager.SqlAzureUsername,
                                                                      _manager.SqlAzurePassword,
                                                                      _manager.SqlAzureLocation)
                                               {
                                                   SubscriptionId = _manager.SubscriptionId,
                                                   Certificate = _manager.ManagementCertificate
                                               };
            _manager.AddNewServerCommand.Execute();
            _manager.SqlAzureServerName = _manager.AddNewServerCommand.SqlAzureServerName;
            return _manager.SqlAzureServerName;
        }

        internal void DeleteSqlServer()
        {
            try
            {
                var command = new DeleteSqlServerCommand(_manager.SqlAzureServerName)
                                  {
                                      SubscriptionId = _manager.SubscriptionId,
                                      Certificate = _manager.ManagementCertificate
                                  };
                command.Execute();
            }
            catch (Exception ex)
            {
                _manager.WriteComplete(EventPoint.SqlAzureServerCreated, ex.Message);
            }
        }

        /// <summary>
        /// Gets a connection to a Sql Database
        /// </summary>
        private SqlConnection GetConnection(string dbName)
        {
            string connectionString =
                String.Format(
                    "server=tcp:{0}.database.windows.net; database={1}; user id={2}@{0}; password={3}; Trusted_Connection=False; Encrypt=True;",
                    _manager.SqlAzureServerName, dbName, _manager.SqlAzureUsername, _manager.SqlAzurePassword);
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Executes a set of commands against a Sql database
        /// </summary>
        private void ExecuteCommand(SqlConnection connection, IEnumerable<string> sql)
        {
            foreach (string sqlSingle in sql)
            {
                var command = new SqlCommand(sqlSingle, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a set of scripts against a Sql Azure database
        /// </summary>
        /// TODO: Test this properly!!!!
        private void ExecuteScripts(SqlConnection connection)
        {
            foreach (string script in _manager.SqlScripts)
            {
                ExecuteBatchNonQuery(script, connection);
                _manager.WriteComplete(EventPoint.SqlAzureScriptsExecutedSuccessfully, "Script executed successfully");
            }
        }

        /* courtesy of Blorgbeard! */
        private void ExecuteBatchNonQuery(string sql, SqlConnection conn)
        {
            string sqlBatch = string.Empty;
            var cmd = new SqlCommand(string.Empty, conn);
            conn.Open();
            sql += "\nGO";   // make sure last batch is executed.
            try
            {
                foreach (string line in sql.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.ToUpperInvariant().Trim() == "GO")
                    {
                        cmd.CommandText = sqlBatch;
                        cmd.ExecuteNonQuery();
                        sqlBatch = string.Empty;
                    }
                    else
                    {
                        sqlBatch += line + "\n";
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        #region Implementation of IServiceTransaction

        /// <summary>
        /// Used to commit the transaction data 
        /// </summary>
        /// <returns>A dynamic type which represents the return of the particular transaction</returns>
        public dynamic Commit()
        {
            try
            {
                _started = _success = true;
                _manager.SqlAzureServerName = _manager.SqlAzureServerName ?? AddNewSqlServer();
                _manager.WriteComplete(EventPoint.SqlAzureServerCreated,
                                       "Sql Azure Server created with name " + _manager.SqlAzureServerName);

                // TODO: This needs to read less like a broken script!
                if (_manager.ActionType == ActionType.Delete)
                {
                    DeleteSqlServer();
                    return true;
                }

                foreach (ISqlAzureFirewallRule rule in _manager.FirewallRules)
                {
                    rule.ConfigureFirewallCommand(_manager.SqlAzureServerName);
                    var command = (ServiceCommand) rule;
                    command.Execute();
                    _manager.WriteComplete(EventPoint.SqlAzureFirewallIpAdded,
                                           String.Format("Rulename {0} added to server {1} for IP range {2} - {3}",
                                                         rule.SqlAzureRuleName, rule.SqlAzureServerName,
                                                         rule.SqlAzureClientIpAddressLow,
                                                         rule.SqlAzureClientIpAddressHigh));
                }

                SqlConnection masterConnection = GetConnection("master");

                // TODO: Replace this with checks to prevent injection attacks
                ExecuteCommand(masterConnection, new[] {"CREATE DATABASE " + _manager.SqlAzureDatabaseName + " (EDITION = 'web')"});
                _manager.WriteComplete(EventPoint.SqlAzureDatabaseCreated, "Created database " + _manager.SqlAzureDatabaseName);
                SqlConnection dbConnection = GetConnection(_manager.SqlAzureDatabaseName);
                foreach (var item in _manager.DbUsers)
                {
                    ExecuteCommand(masterConnection,
                                   new[] {String.Format("CREATE LOGIN {0} WITH PASSWORD = '{1}'", item.Key, item.Value)});
                    ExecuteCommand(dbConnection, new[]
                                                     {
                                                         String.Format("CREATE USER {0} FROM LOGIN {0}", item.Key),
                                                         String.Format("EXEC sp_addrolemember N'db_owner', N'{0}'",
                                                                       item.Key)
                                                     });
                    _manager.WriteComplete(EventPoint.SqlAzureDatabaseAdminCreated,
                                           "User created with login " + item.Key);
                }

                if (_manager.SqlScripts.Count > 0)
                    ExecuteScripts(dbConnection);

                if (masterConnection.State == ConnectionState.Open)
                    masterConnection.Close();
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception exception)
            {
                _success = false;
                _manager.WriteComplete(EventPoint.ExceptionOccurrence, exception.GetType() + ": " + exception.Message);
            }

            return _success;
        }

        /// <summary>
        /// Used to rollback the transaction in the event of failure 
        /// </summary>
        public void Rollback()
        {
            DeleteSqlServer();
        }

        /// <summary>
        /// Used to denote whether the transaction has been started or not
        /// </summary>
        public bool Started
        {
            get { return _started; }
        }

        /// <summary>
        /// Used to denote whether the transaction has succeeded or not
        /// </summary>
        public bool Succeeded
        {
            get { return _success; }
        }

        #endregion
    }
}