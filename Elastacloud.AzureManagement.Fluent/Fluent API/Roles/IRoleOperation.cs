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
    }
}
