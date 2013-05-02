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
    public class WebsiteGet : IExecute
    {
         private readonly string _subscriptionId;
         private readonly X509Certificate2 _certificate;
         private readonly string _websiteName;

        public WebsiteGet(ApplicationFactory factory)
        {
            _subscriptionId = factory.SubscriptionId;
            _certificate = factory.ManagementCertificate;
            _websiteName = factory.CloudServiceName;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            System.Console.WriteLine("reading website");
            System.Console.WriteLine("================");

            var client = new WebsiteClient(_subscriptionId, _certificate);
            var list = client.List();

            list.ForEach(a => System.Console.WriteLine("Website hosts: " + String.Join(", ", a.Hostname.ToArray())));

            //var client2 = new WebsiteClient(_subscriptionId, _certificate, list[0].Name);
            var client2 = new WebsiteClient(_subscriptionId, _certificate, "testfluent39");
            System.Console.WriteLine(client2.WebsiteProperties.Config.DetailedErrorLoggingEnabled);
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
