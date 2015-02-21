using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Watchers
{
    /// <summary>
    /// The watcher class used to monitor whether a role has had a status change
    /// </summary>
    public class DeploymentStatusWatcher : BaseWatcher
    {
        /// <summary>
        /// Used to construct a DeploymentStatusWatcher
        /// </summary>
        /// <param name="serviceName">The name of the hosted service</param>
        /// <param name="slot">Production or Staging</param>
        /// <param name="managementCertificate">the management certificate used with this role</param>
        /// <param name="subscriptionId">The id of the subscription we're interested in</param>
        internal DeploymentStatusWatcher(string serviceName, DeploymentSlot slot, X509Certificate2 managementCertificate, string subscriptionId)
            : base(serviceName, managementCertificate, subscriptionId)
        {
            ProductionOrStaging = slot;
            CurrentState = DeploymentStatus.Unknown;
        }

        /// <summary>
        /// The deployment slot used to define the role
        /// </summary>
        protected DeploymentSlot ProductionOrStaging { get; set; }

        /// <summary>
        /// Returns the current state of the deployment (and by extension role - although not 100% accurate) that we're interested in
        /// </summary>
        public DeploymentStatus CurrentState { get; private set; }

        #region Overrides of BaseWatcher

        /// <summary>
        /// Overriden in derived classes and used to ping back to determine the state
        /// </summary>
        /// <param name="state">an object state containing details of the command response</param>
        protected override void Pingback(object state)
        {
            DeploymentStatus status;
            var command = new GetDeploymentStatusCommand(HostedServiceName, ProductionOrStaging)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            try
            {
                command.Execute();
                status = command.DeploymentStatus;
            }
            catch (WebException webx)
            {
                // put this in if we can't get a reading back from this!!
                status = DeploymentStatus.Unknown;
            }
            
            if (status != CurrentState && RoleStatusChangeHandler != null)
                RoleStatusChangeHandler(this, new CloudServiceState(CurrentState, status));
            CurrentState = status;
        }

        /// <summary>
        /// Used to get the state to feed into the Pingback method
        /// </summary>
        /// <returns>an object to be cast in the derived class</returns>
        protected override object GetState()
        {
            return null;
        }

        #endregion

        /// <summary>
        /// The event that should be subscribed to get role status change information back
        /// </summary>
        public event EventHandler<CloudServiceState> RoleStatusChangeHandler;

    }

    public class CloudServiceState
    {
        public CloudServiceState(DeploymentStatus oldState, DeploymentStatus newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public DeploymentStatus OldState { get; private set; }
        public DeploymentStatus NewState { get; private set; }
    }
}