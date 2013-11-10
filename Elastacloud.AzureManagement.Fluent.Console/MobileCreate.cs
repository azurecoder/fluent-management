using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class MobileCreate : IExecute
    {
        private readonly string _subscriptionId;
        private readonly X509Certificate2 _certificate;
        private readonly string _mobileServiceName;

        public MobileCreate(ApplicationFactory factory)
        {
            _subscriptionId = factory.SubscriptionId;
            _certificate = factory.ManagementCertificate;
            _mobileServiceName = factory.CloudServiceName;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            System.Console.WriteLine("Create mobile service");
            System.Console.WriteLine("=====================");

            var client = new MobileServiceClient(_subscriptionId, _certificate);
            client.CreateMobileServiceWithNewDb("racmobile2", "supersqluser", "Abc48c16");

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
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
