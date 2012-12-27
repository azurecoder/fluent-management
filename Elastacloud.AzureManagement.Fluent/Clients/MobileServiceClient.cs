using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.MobileServices;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// An azure implementation of a mobile service client 
    /// </summary>
    public class MobileServiceClient : IMobileServiceClient
    {
        private readonly List<MobileServiceLogEntry> _logs = new List<MobileServiceLogEntry>();

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
            EnsureMobileServicesName();
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
        /// <param name="defaultPermission">Sets the default permission for the table scripts</param>
        public void AddTable(string tableName, Types.MobileServices.Roles defaultPermission = Types.MobileServices.Roles.Application)
        {
            if(String.IsNullOrEmpty(tableName))
                throw new FluentManagementException("unable to add table with an empty name", "CreateMobileServicesTableCommand");
            var permission = defaultPermission.ToString().ToLower();
            var dictionary = BuildCrudDictionary(new List<string> {permission, permission, permission, permission});
            dictionary["name"] = tableName;
            EnsureMobileServicesName();
            var config = JsonConvert.SerializeObject(dictionary);
            var command = new CreateMobileServiceTableCommand(MobileServiceName, tableName, config)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();
        }

        /// <summary>
        /// Adds a script for a crud operation to the table
        /// </summary>
        /// <param name="operationType">The type of operation</param>
        /// <param name="tableName">The name of the WAMS table</param>
        /// <param name="script">The script to add</param>
        /// <param name="permission">The permissions of the script to upload</param>
        public void AddTableScript(CrudOperation operationType, string tableName, string script, Types.MobileServices.Roles permission)
        {
            if (String.IsNullOrEmpty(tableName))
                throw new FluentManagementException("unable to add table with an empty name", "CreateMobileServicesTableScriptCommand");
            EnsureMobileServicesName();
            Refresh();
            // then create the script
            var command = new CreateMobileServiceTableScriptCommand(MobileServiceName, tableName, operationType, script)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // update the script with the new permissions
            var table = Tables.FirstOrDefault(a => a.TableName == tableName);
            var values = new List<string> { table.InsertPermission.ToString(), table.UpdatePermission.ToString(), table.ReadPermission.ToString(), table.DeletePermission.ToString()};
            // TODO: speak to MSFT about this - the cmdlets have a bug and all of the permissions need to be added for them to update more than a single one

            var dictionary = BuildCrudDictionary(values);
            dictionary[operationType.ToString().ToLower()] = permission.ToString().ToLower();
                                
            // updates the script table service permissions
            var config = JsonConvert.SerializeObject(dictionary);
            var updateCommand = new UpdateMobileServiceTablePermissionsCommand(MobileServiceName, tableName, config)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            updateCommand.Execute();
        }

        private Dictionary<string, string> BuildCrudDictionary(IList<string> values)
        {
            return new Dictionary<string, string>
                                 {
                                     {CrudOperation.Insert.ToString().ToLower(), values[0]},
                                     {CrudOperation.Update.ToString().ToLower(), values[1]},
                                     {CrudOperation.Read.ToString().ToLower(), values[2]},
                                     {CrudOperation.Delete.ToString().ToLower(), values[3]}
                                 };
        }

        /// <summary>
        /// Adds a scheduled job to WAMS
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <param name="script">The actual script</param>
        /// <param name="intervalInMinutes">The interval in minutes</param>
        public void AddSchedulerScript(string name, string script, int intervalInMinutes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates any of the settable prooperties of the mobile service
        /// </summary>
        public void Update()
        {
            UpdateLiveNotifications();
            UpdateAuth();
            UpdateService();
        }
        
        /// <summary>
        /// Refreshes the state of the client
        /// </summary>
        public void Refresh()
        {
            if (MobileServiceName == null) return;
            GetAllSettings();
            GetMobileServiceDetails();
            GetMobileServiceResources();
            GetMobileServiceTables();
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

        /// <summary>
        /// Gets a list of mobile services tables
        /// </summary>
        public List<MobileServiceTable> Tables { get; private set; }

        /// <summary>
        /// Gets or sets whether the dynamic schema is enabled or not
        /// </summary>
        public bool DynamicSchemaEnabled { get; set; }

        /// <summary>
        /// The client secret for live id access
        /// </summary>
        public string MicrosoftAccountClientSecret { get; set; }

        /// <summary>
        /// The client id for live id access
        /// </summary>
        public string MicrosoftAccountClientId { get; set; }

        /// <summary>
        /// The package sid for wns 
        /// </summary>
        public string MicrosoftAccountPackageSID { get; set; }

        /// <summary>
        /// The client id for facebook access
        /// </summary>
        public string FacebookClientId { get; set; }

        /// <summary>
        /// The client secret for facebook access
        /// </summary>
        public string FacebookClientSecret { get; set; }

        /// <summary>
        /// The client id for google access
        /// </summary>
        public string GoogleClientId { get; set; }

        /// <summary>
        /// The client secret for google access
        /// </summary>
        public string GoogleClientSecret { get; set; }

        /// <summary>
        /// The client id for twitter access
        /// </summary>
        public string TwitterClientId { get; set; }

        /// <summary>
        /// The client secret for twitter access
        /// </summary>
        public string TwitterClientSecret { get; set; }

        /// <summary>
        /// The mobile service log entry entries
        /// </summary>
        public List<MobileServiceLogEntry> Logs
        {
            get
            {
                // TODO: speak to mobile services team as date filtering not returning any results for 
                // $filter ge datetime'xTyZ' so had to discard
                var minTime = DateTime.MinValue;
                var logItem = _logs.OrderBy(entry => entry.TimeCreated).FirstOrDefault();
                // if there is no log item then this is the first time that this call has been made - or there are no logs!!
                if (logItem != null)
                    minTime = logItem.TimeCreated;
                // execute the logs command
                var command = new GetMobileServiceLogsCommand(MobileServiceName)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                command.Execute();
                // convert the JSON response and append the log increment
                var results = (JObject)JsonConvert.DeserializeObject(command.JsonResult);
                var logs = results["results"].ToObject<List<MobileServiceLogEntry>>();
                var incrementalLogs = logs.Where(entry => entry.TimeCreated > minTime).ToList();
                _logs.AddRange(incrementalLogs);

                return _logs;
            }
        }

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
            var jjson = GetCreateNewServiceSpecification();
            var json = JsonConvert.DeserializeObject(jjson);

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
        private void GetMobileServiceDetails()
        {
            //execute the details command 
            var details = new GetMobileServiceDetailsCommand(MobileServiceName)
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
        /// Ensures that the mobile services name has a value
        /// </summary>
        private void EnsureMobileServicesName()
        {
            if(String.IsNullOrEmpty(MobileServiceName))
                throw new FluentManagementException("No mobile services name present", "GetMobileServiceResourcesCommand");
        }

        /// <summary>
        /// Gets and populates all of the state of the resources 
        /// </summary>
        private void GetMobileServiceResources()
        {
            //execute the details command 
            var details = new GetMobileServiceResourcesCommand(MobileServiceName)
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

        /// <summary>
        /// Gets a list of all of the mobile services tables
        /// </summary>
        private void GetMobileServiceTables()
        {
            var command = new ListMobileServiceTablesCommand(MobileServiceName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            Tables = command.Tables;
            foreach (var table in Tables)
            {
                var permissionsCommand = new GetMobileServiceTablePermissionCommand(MobileServiceName, table.TableName)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                permissionsCommand.Execute();
                table.InsertPermission = permissionsCommand.InsertPermission;
                table.ReadPermission = permissionsCommand.ReadPermission;
                table.UpdatePermission = permissionsCommand.UpdatePermission;
                table.DeletePermission = permissionsCommand.DeletePermission;
            }
        }

        /// <summary>
        /// Gets a list of mobile auth provider settings and populates 
        /// </summary>
        private void GetMobileAuthenticationProviderSettings()
        {
            // execute the command 
            var command = new GetMobileServiceSettingsCommand(MobileServiceName, Constants.MobileServicesAuthSettings)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // get all the mobile providers returned 
            var mobileProviders = (List<MobileServicesAuthProvider>)JsonConvert.DeserializeObject(command.JsonResult, typeof(List<MobileServicesAuthProvider>));
            foreach (var mobileServicesAuthProvider in mobileProviders)
            {
                switch (mobileServicesAuthProvider.Provider)
                {
                    case Constants.GoogleProvider:
                        GoogleClientId = mobileServicesAuthProvider.AppId;
                        GoogleClientSecret = mobileServicesAuthProvider.Secret;
                        break;
                    case Constants.FacebookProvider:
                        FacebookClientId = mobileServicesAuthProvider.AppId;
                        FacebookClientSecret = mobileServicesAuthProvider.Secret;
                        break;
                    case Constants.TwitterProvider:
                        TwitterClientId = mobileServicesAuthProvider.AppId;
                        TwitterClientSecret = mobileServicesAuthProvider.Secret;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets all of the Windows settings for notifications and other things
        /// </summary>
        private void GetLiveNotificationSettings()
        {
            // execute the command 
            var command = new GetMobileServiceSettingsCommand(MobileServiceName, Constants.MobileServicesLiveSettings)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // get all the mobile providers returned 
            var windows = (WindowsAuthProvider)JsonConvert.DeserializeObject(command.JsonResult, typeof(WindowsAuthProvider));
            MicrosoftAccountClientId = windows.ClientId;
            MicrosoftAccountClientSecret = windows.ClientSecret;
            MicrosoftAccountPackageSID = windows.PackageSid;
        }

        /// <summary>
        /// Gets all of the Windows settings for notifications and other things
        /// </summary>
        private void GetServiceSettings()
        {
            // execute the command 
            var command = new GetMobileServiceSettingsCommand(MobileServiceName, Constants.MobileServicesServiceSettings)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // get all the mobile providers returned 
            
            var settings = (JObject)JsonConvert.DeserializeObject(command.JsonResult);
            DynamicSchemaEnabled = (bool)settings.GetValue(Constants.DynamicSchemaEnabled).ToObject(typeof(bool));
        }

        /// <summary>
        /// Gets an aggregate of all of the mobile services settings
        /// </summary>
        private void GetAllSettings()
        {
            GetServiceSettings();
            GetLiveNotificationSettings();
            GetMobileAuthenticationProviderSettings();
        }

        #endregion

        #region Updates

        /// <summary>
        /// Used to update the authentication for the mobile service
        /// </summary>
        private void UpdateAuth()
        {
            // setup the providers
            var providers = new List<MobileServicesAuthProvider>();
            var googleProvider = new MobileServicesAuthProvider(Constants.GoogleProvider, GoogleClientId, GoogleClientSecret);
            var facebookProvider = new MobileServicesAuthProvider(Constants.FacebookProvider, FacebookClientId, FacebookClientSecret);
            var twitterProvider = new MobileServicesAuthProvider(Constants.TwitterProvider, TwitterClientId, TwitterClientSecret);
            //check whether they are empty or not
            if (!(String.IsNullOrEmpty(GoogleClientId) && String.IsNullOrEmpty(GoogleClientSecret)))
            {
                providers.Add(googleProvider);
            }
            if (!(String.IsNullOrEmpty(TwitterClientId) && String.IsNullOrEmpty(TwitterClientSecret)))
            {
                providers.Add(twitterProvider);
            }
            if (!(String.IsNullOrEmpty(FacebookClientId) && String.IsNullOrEmpty(FacebookClientSecret)))
            {
                providers.Add(facebookProvider);
            }
            // execute the command
            var converted = JsonConvert.SerializeObject(providers);
            var command = new UpdateMobileServiceSettingsCommand(MobileServiceName, Constants.MobileServicesAuthSettings, converted)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
        }

        /// <summary>
        /// Used to update the service settings currently dynamic schema only
        /// </summary>
        private void UpdateService()
        {
            // we'll always have a value for this 
            var dictionary = new Dictionary<string, string>();
            dictionary[Constants.DynamicSchemaEnabled] = DynamicSchemaEnabled.ToString().ToLower();
            var converted = JsonConvert.SerializeObject(dictionary);
            // execute this command
            // TODO: speak to MSFT the current verb is PATCH it would be good to understand where this is going
            var command = new UpdateMobileServiceSettingsCommand(MobileServiceName, Constants.MobileServicesServiceSettings, converted, "PATCH")
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
        }

        /// <summary>
        /// Used to update everything to do with a Microsoft Account including WNS
        /// </summary>
        private void UpdateLiveNotifications()
        {
            // set this up 
            int count = 3;
            if (String.IsNullOrEmpty(MicrosoftAccountClientId)) count--;
            if (String.IsNullOrEmpty(MicrosoftAccountClientSecret)) count--;
            if (String.IsNullOrEmpty(MicrosoftAccountPackageSID)) count--;
            // if we only have a single value we're interested
            // permutations are client id + secret OR client id + package sid
            if (count < 2) return;
            var converted = JsonConvert.SerializeObject(new WindowsAuthProvider(MicrosoftAccountPackageSID, MicrosoftAccountClientId, MicrosoftAccountClientSecret));
            // execute the command
            var command = new UpdateMobileServiceSettingsCommand(MobileServiceName, Constants.MobileServicesLiveSettings, converted)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
        }

        #endregion

    }
}