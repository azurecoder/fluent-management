using System;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// Used to update the state of a wasabi web update
    /// </summary>
    public delegate void ScaleStateChange(WasabiWebState state, int newScaleCount);
    /// <summary>
    /// Generates an alert if a metric has breached one of the rules
    /// </summary>
    public delegate void MetricAlert(WebsiteMetric metric, IWasabiWebRule rule);
    /// <summary>
    /// Used to connect to the service management API through fluent management
    /// </summary>
    public interface IWebsiteManagementConnector
    {
        /// <summary>
        /// Subscribes to the alerts if a metric threshold has been breached
        /// </summary>
        event MetricAlert SubscribeAlerts;
        /// <summary>
        /// Fires when the timer fires with a scale state change
        /// </summary>
        event ScaleStateChange ScaleUpdate;
        /// <summary>
        /// Updates the website based on the scale option 
        /// </summary>
        WasabiWebState MonitorAndScale();
        /// <summary>
        /// Raises the alert event if any of the metrics have been breached in the time period
        /// </summary>
        void MonitorAndAlert();
        /// <summary>
        /// The subscription id for the user's account
        /// </summary>
        string SubscriptionId { get; set; }
        /// <summary>
        /// Sets the publishsettings file so that it can be 
        /// </summary>
        string PublishSettingsFile { get; set; }
        /// <summary>
        /// The X509v2 management certificate
        /// </summary>
        X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// The type of logical operation to perform on the rules
        /// </summary>
        WasabiWebLogicalOperation Operation { get; set; }
    }
}
