using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Watchers;

namespace Elastacloud.AzureManagement.Fluent.Roles
{
    public class RoleContextManager : IRoleOperation
    {
        private RoleStatus _status = RoleStatus.Unknown;
        private RoleStatusWatcher _watcher;
        /// <summary>
        /// Used to create a role context
        /// </summary>
        internal RoleContextManager(string subscriptionId, X509Certificate2 certificate, string serviceName, DeploymentSlot slot)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            ServiceName = serviceName;
            DeploymentSlot = slot;
        }

        protected X509Certificate2 ManagementCertificate { get; set; }
        protected string SubscriptionId { get; set; }
        protected DeploymentSlot DeploymentSlot { get; set; }
        protected string ServiceName { get; set; }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        #endregion

        #region Implementation of IRoleOperation

        /// <summary>
        /// Used to start a deployment
        /// </summary>
        public void Start()
        {
            var command = new UpdateRoleStatusCommand(ServiceName, DeploymentSlot, UpdateDeploymentStatus.Running)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            command.Execute();
        }

        /// <summary>
        /// Used to stop a deployment
        /// </summary>
        public void Stop()
        {
            var command = new UpdateRoleStatusCommand(ServiceName, DeploymentSlot, UpdateDeploymentStatus.Suspended)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
        }

        /// <summary>
        /// Used to update the number of instances for an existing role
        /// </summary>
        /// <param name="roleName">The name of the role</param>
        /// <param name="instanceCount">the number of instances to increment or decrement to</param>
        public void UpdateInstanceCountForRole(string roleName, int instanceCount)
        {
            var config = GetConfiguration(roleName);
            config.SetInstanceCountForRole(roleName, instanceCount);

            var commandSetter = new SetDeploymenConfigurationCommand(ServiceName, config, DeploymentSlot)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            commandSetter.Execute();
        }

        /// <summary>
        /// Returns the configuration file for the particular role
        /// </summary>
        private CscfgFile GetConfiguration(string roleName)
        {
            RoleName = roleName;
            // get the details configuration for the role
            var command = new GetDeploymenConfigurationCommand(ServiceName, DeploymentSlot)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            command.Execute();
            // use the configuration and reset the instance count for the role in question
            var config = command.Configuration;
            return config;
        }

        /// <summary>
        /// Returns the instance count for the role being queried
        /// </summary>
        public int GetInstanceCountForRole(string roleName)
        {
            var config = GetConfiguration(roleName);
            return config.GetInstanceCountForRole(roleName);
        }

        #endregion

        /// <summary>
        /// This is the role that is being used for the update
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets the current status of the role
        /// </summary>
        public RoleStatus CurrentStatus
        {
            get
            {
                if(_watcher == null)
                    _watcher = new RoleStatusWatcher(ServiceName, RoleName, DeploymentSlot, ManagementCertificate, SubscriptionId);

                return _watcher.CurrentState;
            }
        }
    }
}
