using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Roles
{
    public class RoleContextManager : IRoleOperation
    {
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
                Certificate = null
            };
            command.Execute();
        }

        #endregion

        public string RoleName { get; set; }
    }
}
