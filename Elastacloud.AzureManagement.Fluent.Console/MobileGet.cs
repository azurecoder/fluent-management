using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Console.Properties;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class MobileGet : IExecute
    {
         private readonly string _subscriptionId;
         private readonly X509Certificate2 _certificate;
         private readonly string _mobileServiceName;

        public MobileGet(string subscriptionId, X509Certificate2 certificate, string mobileServiceName)
        {
            _subscriptionId = subscriptionId;
            _certificate = certificate;
            _mobileServiceName = mobileServiceName;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            var client = new MobileServiceClient(_subscriptionId, _certificate, _mobileServiceName);
            if(!client.Tables.Exists(a => a.TableName == "Speakers"))
                client.AddTable("Speakers");
            client.AddTableScript(CrudOperation.Insert, "Speakers", Resources.insert_js, Types.MobileServices.Roles.Public);
            client.FacebookClientId = "test";
            client.FacebookClientSecret = "test";
            client.GoogleClientId = "test";
            client.GoogleClientSecret = "test";
            client.TwitterClientId = "test";
            client.TwitterClientSecret = "test";
            client.DynamicSchemaEnabled = true;
            client.MicrosoftAccountClientId = "test";
            client.MicrosoftAccountClientSecret = "test";
            client.Update();

            System.Console.WriteLine("Application key: {0}", client.ApplicationKey);
            System.Console.WriteLine("Application url: {0}", client.ApplicationUrl);
            System.Console.WriteLine("Description: {0}", client.Description);
            System.Console.WriteLine("Location: {0}", client.Location);
            System.Console.WriteLine("Master key: {0}", client.MasterKey);
            System.Console.WriteLine("Mobile service Db name: {0}", client.MobileServiceDbName);
            System.Console.WriteLine("Mobile service server name: {0}", client.MobileServiceSqlName);
            System.Console.WriteLine("Sql Azure Db name: {0}", client.SqlAzureDbName);
            System.Console.WriteLine("Sql Azure server name: {0}", client.SqlAzureServerName);
            System.Console.WriteLine("Mobile service state: {0}", client.MobileServiceState.ToString());

            foreach (var table in client.Tables)
            {
                System.Console.WriteLine("==================================================");
                System.Console.WriteLine("Table name: {0}", table.TableName);
                System.Console.WriteLine("Size: {0} bytes", table.SizeInBytes);
                System.Console.WriteLine("Row count: {0}", table.NumberOfRecords);
                System.Console.WriteLine("No of indexes: {0}", table.NumberOfIndexes);
                System.Console.WriteLine("Read permission: {0}", table.ReadPermission);
                System.Console.WriteLine("Insert permission: {0}", table.InsertPermission);
                System.Console.WriteLine("Delete permission: {0}", table.DeletePermission);
                System.Console.WriteLine("Update permission: {0}", table.UpdatePermission);
                System.Console.WriteLine("==================================================");
            }

            var logs = client.Logs;
            System.Console.WriteLine("There are {0} log entries", logs.Count);
            System.Console.WriteLine("There are {0} error log entries", logs.Where(a => a.Type == LogLevelType.Error));
            System.Console.WriteLine("There are {0} warning log entries", logs.Where(a => a.Type == LogLevelType.Warning));
            System.Console.WriteLine("There are {0} information log entries", logs.Where(a => a.Type == LogLevelType.Information));
        }

        #endregion
    }
}
