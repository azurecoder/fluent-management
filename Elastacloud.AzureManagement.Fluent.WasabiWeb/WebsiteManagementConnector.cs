using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        private X509Certificate2 _managementCertificate;
        /// <summary>
        /// Used to construct a connector to Fluent Management
        /// </summary>
        public WebsiteManagementConnector(IWasabiWebRulesEngine engine)
        {
            _engine = engine;
            // build the timer here using the samples period
            var timer = new Timer(engine.SamplesPeriodInMins*60*1000)
                {
                    AutoReset = true,
                    Enabled = true,
                };
            timer.Elapsed += (sender, args) => _stateHistory.Add(DateTime.Now, Update());
        }

        #region Implementation of IWebsiteManagementConnector

        /// <summary>
        /// Updates the website based on the scale option 
        /// </summary>
        public WasabiWebState Update()
        {
            // get the management certificate first of all
            if (_managementCertificate == null)
            {
                if(String.IsNullOrEmpty(PublishSettingsFile) && String.IsNullOrEmpty(SubscriptionId))
                    throw new ApplicationException("Unable to find publishsettings files or subscription id is empty");

                var settings = new PublishSettingsExtractor(PublishSettingsFile);
                _managementCertificate = settings.AddPublishSettingsToPersonalMachineStore();
            }

            // use the certificate to run the client and command 
            var client = new WebsiteClient(SubscriptionId, _managementCertificate, _engine.WebsiteName);
            // get the metrics for the timer time period
            var metrics = client.GetWebsiteMetricsPerInterval(TimeSpan.FromMinutes(_engine.SamplesPeriodInMins));
            var scalePotential = _engine.Scale(WasabiWebLogicalOperation.And, metrics);
            // with the scale potential we'll need to increase or decrease the instance count
            if (scalePotential == WasabiWebState.ScaleDown && client.InstanceCount > 1)
                client.InstanceCount -= 1;
            if (scalePotential == WasabiWebState.ScaleUp && client.InstanceCount < 10)
                client.InstanceCount += 1;
            client.Update();

            return scalePotential;
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