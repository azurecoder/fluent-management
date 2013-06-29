using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Timers;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// An implementation of the custom connector which uses Fluent Management
    /// </summary>
    public class WebsiteManagementConnector : IWebsiteManagementConnector
    {
        /// <summary>
        /// An empty timer used to callback when the sample period is reached
        /// </summary>
        private readonly Timer _timer = null;
        /// <summary>
        /// The state history stored for each of the scaling activities
        /// </summary>
        private readonly Dictionary<DateTime, WasabiWebState> _stateHistory = new Dictionary<DateTime, WasabiWebState>();
        /// <summary>
        /// The engine that is being used to drive the calculations on scale
        /// </summary>
        private readonly IWasabiWebRulesEngine _engine;
        /// <summary>
        /// The X509v2 management certificate
        /// </summary>
        public X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// The type of logical operation to perform on the rules
        /// </summary>
        public WasabiWebLogicalOperation Operation { get; set; }
        /// <summary>
        /// Used to construct a connector to Fluent Management
        /// </summary>
        public WebsiteManagementConnector(IWasabiWebRulesEngine engine, string subscriptionId, WasabiWebLogicalOperation logicalOperation = WasabiWebLogicalOperation.And, 
            string publishSettingsFile = null)
        {
            _engine = engine;
            SubscriptionId = subscriptionId;
            PublishSettingsFile = publishSettingsFile;
            Operation = logicalOperation;
            // build the timer here using the samples period
            _timer = new Timer(engine.SamplesPeriodInMins*60*1000)
                {
                    AutoReset = true,
                    Enabled = true,
                };
        }

        #region Implementation of IWebsiteManagementConnector

        /// <summary>
        /// Used when alerts are subscribed to
        /// </summary>
        public event MetricAlert SubscribeAlerts;

        /// <summary>
        /// The Scale change event which is raised when the service completes
        /// </summary>
        public event ScaleStateChange ScaleUpdate;

        /// <summary>
        /// Updates the website based on the scale option 
        /// </summary>
        public WasabiWebState MonitorAndScale()
        {
            _timer.Elapsed += (sender, args) => _stateHistory.Add(DateTime.Now, MonitorAndScale());

            EnsureManagementCertificate();

            // use the certificate to run the client and command 
            var client = new WebsiteClient(SubscriptionId, ManagementCertificate, _engine.WebsiteName);
            // get the metrics for the timer time period
            var metrics = client.GetWebsiteMetricsPerInterval(TimeSpan.FromMinutes(_engine.SamplesPeriodInMins));
            if(metrics.Count == 0)
                return WasabiWebState.LeaveUnchanged;

            var scalePotential = _engine.Scale(Operation, metrics);
            // with the scale potential we'll need to increase or decrease the instance count
            if (scalePotential == WasabiWebState.ScaleDown && client.InstanceCount > 1)
                client.InstanceCount -= 1;
            if (scalePotential == WasabiWebState.ScaleUp && client.InstanceCount < 10)
                client.InstanceCount += 1;
            client.Update();
            // raise the event now
            if(ScaleUpdate != null)
                ScaleUpdate(scalePotential, client.InstanceCount);

            return scalePotential;
        }
        // check to see whether there is a management certificate and use it in the service management call
        private void EnsureManagementCertificate()
        {
            // get the management certificate first of all
            if (ManagementCertificate == null)
            {
                if (String.IsNullOrEmpty(PublishSettingsFile) && String.IsNullOrEmpty(SubscriptionId))
                    throw new ApplicationException("Unable to find publishsettings files or subscription id is empty");

                var settings = new PublishSettingsExtractor(PublishSettingsFile);
                ManagementCertificate = settings.AddPublishSettingsToPersonalMachineStore();
            }
        }

        /// <summary>
        /// Raises the alert event if any of the metrics have been breached in the time period
        /// </summary>
        public void MonitorAndAlert()
        {
            EnsureManagementCertificate();

            // use the certificate to run the client and command 
            var client = new WebsiteClient(SubscriptionId, ManagementCertificate, _engine.WebsiteName);
            // get the metrics for the timer time period
            var metrics = client.GetWebsiteMetricsPerInterval(TimeSpan.FromMinutes(_engine.SamplesPeriodInMins));
            // enumerate the metrics piece by piece
            foreach (var metric in metrics)
            {
                // check for the metric to ensure it's in the collection otherwise discard it
                if (_engine[metric.Name] == null)
                    continue;
                var rule = _engine[metric.Name];
                if ((metric.Total > rule.IsGreaterThan || metric.Total < rule.IsLessThan) && SubscribeAlerts != null)
                {
                    SubscribeAlerts(metric, rule);
                }
            }
        }

        /// <summary>
        /// The subscription id for the user's account
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Sets the publishsettings file so that it can be 
        /// </summary>
        public string PublishSettingsFile { get; set; }

        #endregion
    }
}