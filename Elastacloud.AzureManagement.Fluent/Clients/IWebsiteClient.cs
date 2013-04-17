using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// The interface used to roll out websites 
    /// </summary>
    public interface IWebsiteClient
    {
        /// <summary>
        /// Returns a list of websites
        /// </summary>
        List<Website> List();
    }
}