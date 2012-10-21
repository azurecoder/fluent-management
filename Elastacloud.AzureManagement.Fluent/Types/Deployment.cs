using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// Used to refer to a deployment under a cloud service
    /// </summary>
    public class Deployment
    {
        /// <summary>
        /// The name of the deployment
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The slot taken by the deployment
        /// </summary>
        public DeploymentSlot Slot { get; set; }
        /// <summary>
        /// The total role instance belonging to the deployment
        /// </summary>
        public int TotalRoleInstanceCount { get; set; } 
    }
}
