using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public enum WorkerSize
    {
        /// <summary>
        /// The website is in the stopped state
        /// </summary>
        Small,
        /// <summary>
        /// The website is running
        /// </summary>
        Medium,
        Large
    }
}