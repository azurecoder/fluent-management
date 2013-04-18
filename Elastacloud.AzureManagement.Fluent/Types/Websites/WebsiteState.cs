using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public enum WebsiteState
    {
        /// <summary>
        /// The website is in the stopped state
        /// </summary>
        Stopped,
        /// <summary>
        /// The website is running
        /// </summary>
        Running
    }
}