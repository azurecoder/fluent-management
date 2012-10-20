using System.Security.Cryptography.X509Certificates;
using System.Timers;

namespace Elastacloud.AzureManagement.Fluent.Watchers
{
    /// <summary>
    /// The BaseWatcher used to abstract the timer and usage of the command
    /// </summary>
    public abstract class BaseWatcher
    {
        /// <summary>
        /// The threading timer set to an immediate start and 5 second intervals 
        /// </summary>
        protected Timer Timer;

        /// <summary>
        /// Constructor used to start the timer bound to the command
        /// </summary>
        /// <param name="hostedServiceName">Name of the hosted service being watched</param>
        /// <param name="managementCertificate">the management certificate used to request sign</param>
        /// <param name="subscriptionId">the subscription id we're interested in</param>
        protected BaseWatcher(string hostedServiceName, X509Certificate2 managementCertificate, string subscriptionId)
        {
            HostedServiceName = hostedServiceName;
            ManagementCertificate = managementCertificate;
            SubscriptionId = subscriptionId;
            Timer = new Timer(15000)
                        {
                            AutoReset = true,
                            Enabled = true
                        };
            Timer.Elapsed += (sender, args) => Pingback(GetState());
            Timer.Start();
        }

        /// <summary>
        /// The X509v3 management certificate used to manage the watcher
        /// </summary>
        protected X509Certificate2 ManagementCertificate { get; set; }

        /// <summary>
        /// the subscription we're interested in 
        /// </summary>
        protected string SubscriptionId { get; set; }

        /// <summary>
        /// The name of the hosted service being used by the watcher
        /// </summary>
        public string HostedServiceName { get; set; }

        /// <summary>
        /// Overriden in derived classes and used to ping back to determine the state
        /// </summary>
        /// <param name="state">an object state containing details of the command response</param>
        protected abstract void Pingback(object state);

        /// <summary>
        /// Used to get the state to feed into the Pingback method
        /// </summary>
        /// <returns>an object to be cast in the derived class</returns>
        protected abstract object GetState();
    }
}