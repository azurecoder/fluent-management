using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.MobileServices;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// An azure implementation of a mobile service client 
    /// </summary>
    public class MobileServiceClient : IMobileServiceClient
    {
        /// <summary>
        /// Used to construct a mobile service client
        /// </summary>
        /// <param name="subscriptionId">the subscription id referenced</param>
        /// <param name="certificate">The management certificate to access the subscription</param>
        /// <param name="mobileServiceName">The name of the mobile service</param>
        public MobileServiceClient(string subscriptionId, X509Certificate2 certificate, string mobileServiceName = null)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            MobileServiceSqlName = "ZumoSqlServer_" + Guid.NewGuid().ToString("n");
            MobileServiceDbName = "ZumoSqlDatabase_" + Guid.NewGuid().ToString("n");
            MobileServiceName = mobileServiceName;
            Refresh();
        }

        /// <summary>
        /// A management certificate to access a subscription
        /// </summary>
        protected X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// The subscription id we're interested in
        /// </summary>
        protected string SubscriptionId { get; set; }

        #region Implementation of IMobileServiceClient

        /// <summary>
        /// Used to create a new mobile service
        /// </summary>
        /// <param name="serviceName">The name of the mobile service</param>
        /// /// <param name="sqlUsername">the name of the sql user</param>
        /// <param name="sqlPassword">The sql password</param>
        public void CreateMobileServiceWithNewDb(string serviceName, string sqlUsername, string sqlPassword)
        {
            MobileServiceName = serviceName;
            SqlAzureUsername = sqlUsername;
            SqlAzurePassword = sqlPassword;
            MobileServiceCommand command = new CreateMobileServiceCommand(serviceName, null)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            BuildBase64Config(ref command);
            command.Execute();
            Refresh();
        }

        /// <summary>
        /// Adds a table to mobile services
        /// </summary>
        /// <param name="tableName">the name of the table</param>
        public void AddTable(string tableName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a script for a crud operation to the table
        /// </summary>
        /// <param name="operationType">The type of operation</param>
        /// <param name="script"></param>
        public void AddTableString(CrudOperation operationType, string script)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds or replace the Microsoft account credentials for the mobile service
        /// </summary>
        public void AddOrReplaceMicrosoftAccountCredentials(string appKey, string appSecret)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds or replaces the google account credentials for the mobile service
        /// </summary>
        public void AddOrReplaceGoogleAccountCredentials(string appKey, string appSecret)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds or replaces facebook credentials for the mobile service
        /// </summary>
        public void AddOrReplaceFacebookCredentials(string appKey, string appSecret)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds or replaces Yahoo credentials for the mobile service
        /// </summary>
        public void AddOrReplaceYahooCredentials(string appKey, string appSecret)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Turns the dynamic schema on or off
        /// </summary>
        /// <param name="schemaOn">A bool denoting whether the schema is on or off</param>
        public void UpdateDynamicSchema(bool schemaOn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds or replaces WPNS/WNS for the mobile service
        /// </summary>
        public void AddOrReplaceWindowsPushNotificationCredentials(string clientSecret, string packageSid)
        {
            throw new NotImplementedException();
        }

        #region Properties 

        /// <summary>
        /// The mobile service account key
        /// </summary>
        public string ApplicationKey { get; private set; }

        /// <summary>
        /// The secret used to access the mobile service
        /// </summary>
        public string MasterKey { get; private set; }
        
        /// <summary>
        /// The Url of the application
        /// </summary>
        public string ApplicationUrl { get; private set; }

        /// <summary>
        /// Used to get or set a description of the mobile service
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The location of the mobile service
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The name of the Sql Azure server
        /// </summary>
        public string SqlAzureServerName { get; private set; }

        /// <summary>
        /// The name of the Sql Azure database
        /// </summary>
        public string SqlAzureDbName { get; private set; }

        /// <summary>
        /// The username that is used to access the sql azure db
        /// </summary>
        public string SqlAzureUsername { get; set; }

        /// <summary>
        /// The password that is used to access the Sqlazure db
        /// </summary>
        public string SqlAzurePassword { get; set; }

        /// <summary>
        /// The name of the mobile service sql name
        /// string refName = "ZumoSqlServer_" + Guid.NewGuid().ToString("n");
        /// </summary>
        public string MobileServiceSqlName { get; private set; }

        /// <summary>
        /// The name of the mobile sevrice Db name
        /// string refName = "ZumoSqlDatabase_" + Guid.NewGuid().ToString("n");
        /// </summary>
        public string MobileServiceDbName { get; private set; }

        /// <summary>
        /// The name of the mobile service in context
        /// </summary>
        public string MobileServiceName { get; private set; }

        /// <summary>
        /// Gets the state of the mobile service
        /// </summary>
        public State MobileServiceState { get; private set; }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds the configuration necessary to build a mobile service
        /// </summary>
        /// <param name="command">The name of the command</param>
        /// <returns>A base64 string of the properties for configuring the Db with the mobile service</returns>
        private void BuildBase64Config(ref MobileServiceCommand command)
        {
            // make sure that the database details are intact
            //if (String.IsNullOrEmpty(SqlAzureDbName) || String.IsNullOrEmpty(SqlAzureServerName) ||
            //    String.IsNullOrEmpty(SqlAzureUsername) || String.IsNullOrEmpty(SqlAzurePassword))
            //{
            //    throw new FluentManagementException("database details are not present - cannot build fluent management mobile service", command.ToString());
            //}
            var jjson = GetCreateNewServiceSpecification();
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject(jjson);

            command.Config = Convert.ToBase64String(Encoding.UTF8.GetBytes(json.ToString()));
        }

        /// <summary>
        /// Used to package the JSON request
        /// </summary>
        /// <returns>A mobile service spec</returns>
        private string GetCreateNewServiceSpecification()
        {
            return String.Format(Constants.MobileServicesCreateNewTemplate, Constants.MobileServicesSchemaVersion, Constants.MobileServicesSchemaLocation,
                                 "", "", MobileServiceName, Location ?? LocationConstants.NorthEurope, MobileServiceSqlName, MobileServiceDbName,
                                 SqlAzureUsername, SqlAzurePassword, Constants.MobileServicesVersion, Constants.MobileServicesName, Constants.MobileServicesType);
        }

        /// <summary>
        /// Gets the details of the mobile service on startup
        /// </summary>
        /// <param name="name">the name of the service</param>
        private void GetMobileServiceDetails(string name)
        {
            //execute the details command 
            var details = new GetMobileServiceDetailsCommand(name)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            details.Execute();
            //set all of the details in the client
            ApplicationKey = details.ApplicationKey;
            ApplicationUrl = details.ApplicationUrl;
            Location = details.Location;
            MasterKey = details.MasterKey;
        }
        /// <summary>
        /// Refreshes the state of the client
        /// </summary>
        private void Refresh()
        {
            if (MobileServiceName == null) return;
            GetMobileServiceDetails(MobileServiceName);
            GetMobileServiceResources(MobileServiceName);
        }

        /// <summary>
        /// Gets and populates all of the state of the resources 
        /// </summary>
        /// <param name="mobileServiceName">The name of the mobile service</param>
        private void GetMobileServiceResources(string mobileServiceName)
        {
            //execute the details command 
            var details = new GetMobileServiceResourcesCommand(mobileServiceName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            details.Execute();
            //set all of the details in the client
            SqlAzureDbName = details.DatabaseName;
            SqlAzureServerName = details.ServerName;
            MobileServiceSqlName = details.MobileServiceServerName;
            MobileServiceDbName = details.MobileServiceDatabaseName;
            Description = details.Description;
            MobileServiceState = details.State;
        }

        #endregion

    }
}