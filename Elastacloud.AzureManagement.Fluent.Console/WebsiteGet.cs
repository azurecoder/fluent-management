using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Console.Properties;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;
using Elastacloud.AzureManagement.Fluent.Types.Websites;
using Elastacloud.AzureManagement.Fluent.WasabiWeb;

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

            var client3 = new WebsiteClient(_subscriptionId, _certificate, "ukwaug");
            System.Console.WriteLine(client3.WebsiteProperties.Config.DetailedErrorLoggingEnabled);
            var metrics = client3.GetWebsiteMetricsPerInterval(TimeSpan.FromMinutes(600));
            metrics.ForEach(a => System.Console.WriteLine("Name: {0}, Value: {1} {2}", a.Name, a.Total, a.Units));

            var engine = new WasabiWebRulesEngine("ukwaug", 5);
            engine.AddRule(new WasabiWebRule(MetricsConstants.BytesReceived, 10000000, 10000000));
            engine.AddRule(new WasabiWebRule(MetricsConstants.CpuTime, 560000, 5600000));
            var connector = new WebsiteManagementConnector(engine, _subscriptionId, WasabiWebLogicalOperation.Or)
                {
                    ManagementCertificate = _certificate
                };
            connector.ScaleUpdate += (state, count) => System.Console.WriteLine("State: {0}, Scale: {1}", state, count);
            connector.MonitorAndScale();

            var engineAlert = new WasabiWebRulesEngine("ukwaug", 5);
            engineAlert.AddRule(new WasabiWebRule(MetricsConstants.Http2xx, 20));
            var connector2 = new WebsiteManagementConnector(engineAlert, _subscriptionId, WasabiWebLogicalOperation.Or)
            {
                ManagementCertificate = _certificate
            };
            connector2.SubscribeAlerts +=
                (metric, rule) =>
                System.Console.WriteLine("Name: {0}, value: {1} {2}", metric.Name, metric.Total, metric.Units);
            connector2.MonitorAndAlert();
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
