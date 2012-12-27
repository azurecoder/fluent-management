using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Clients
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
        void AddTable(string tableName);
        /// <summary>
        /// Adds a script for a crud operation to the table
        /// </summary>
        /// <param name="operationType">The type of operation</param>
        /// <param name="script"></param>
        void AddTableString(CrudOperation operationType, string script);
        /// <summary>
        /// Adds or replace the Microsoft account credentials for the mobile service
        /// </summary>
        void AddOrReplaceMicrosoftAccountCredentials(string appKey, string appSecret);
        /// <summary>
        /// Adds or replaces the google account credentials for the mobile service
        /// </summary>
        void AddOrReplaceGoogleAccountCredentials(string appKey, string appSecret);
        /// <summary>
        /// Adds or replaces facebook credentials for the mobile service
        /// </summary>
        void AddOrReplaceFacebookCredentials(string appKey, string appSecret);
        /// <summary>
        /// Adds or replaces Yahoo credentials for the mobile service
        /// </summary>
        void AddOrReplaceYahooCredentials(string appKey, string appSecret);
        /// <summary>
        /// Turns the dynamic schema on or off
        /// </summary>
        /// <param name="schemaOn">A bool denoting whether the schema is on or off</param>
        void UpdateDynamicSchema(bool schemaOn);
        /// <summary>
        /// Adds or replaces WPNS/WNS for the mobile service
        /// </summary>
        void AddOrReplaceWindowsPushNotificationCredentials(string clientSecret, string packageSid);
        /// <summary>
        /// The mobile service account key
        /// </summary>
        string ApplicationKey { get; }
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
    }
}