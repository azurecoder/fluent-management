using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Console.Properties;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class WebsiteCreate : IExecute
    {
         private readonly string _subscriptionId;
         private readonly X509Certificate2 _certificate;
         private readonly string _websiteName;

        public WebsiteCreate(ApplicationFactory factory)
        {
            _subscriptionId = factory.SubscriptionId;
            _certificate = factory.ManagementCertificate;
            _websiteName = factory.CloudServiceName;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            var site = new Website()
                           {
                               ComputeMode = ComputeMode.Dedicated,
                               Name = "fluentwebtest38",
                               WebsiteParameters = new WebsiteParameters()
                                                       {
                                                           CurrentNumberOfWorkers = 1
                                                       }
                           };
            site.Config.AppSettings.Add("test1", "test2");
            site.Config.HttpLoggingEnabled = true;
            site.Config.RequestTracingEnabled = true;
            site.Config.DetailedErrorLoggingEnabled = true;
            site.Config.ConnectionStrings.Add(new ConnStringInfo(){Name="test", ConnectionString = "test", Type="SQLAzure"});
            System.Console.WriteLine("Creating website");
            System.Console.WriteLine("================");

            var client = new WebsiteClient(_subscriptionId, _certificate);
            client.CreateFromGithub(site, new GitDetails() { Username = "azurecoder", Password = "changeme", RepositoryName = "test-sample-for-azure-deployments" });
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
