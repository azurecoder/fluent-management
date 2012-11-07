using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Roles
{
    /// <summary>
    /// Used to define the operations that can take place on the role
    /// </summary>
    public interface IRoleOperation : IDisposable
    {
        /// <summary>
        /// Used to start a role
        /// </summary>
        void Start();
        /// <summary>
        /// Used to stop a role
        /// </summary>
        void Stop();
        /// <summary>
        /// Used to update the number of instances for an existing role
        /// </summary>
        /// <param name="roleName">The name of the role</param>
        /// <param name="instanceCount">the number of instances to increment or decrement to</param>
        void UpdateInstanceCountForRole(string roleName, int instanceCount);
    }
}
