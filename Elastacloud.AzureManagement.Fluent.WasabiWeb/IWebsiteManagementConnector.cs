using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// Used to connect to the service management API through fluent management
    /// </summary>
    public interface IWebsiteManagementConnector
    {
        /// <summary>
        /// Updates the website based on the scale option 
        /// </summary>
        WasabiWebState Update();
        /// <summary>
        /// The subscription id for the user's account
        /// </summary>
        string SubscriptionId { get; set; }
        /// <summary>
        /// Sets the publishsettings file so that it can be 
        /// </summary>
        string PublishSettingsFile { get; set; }
    }
}
