using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    /// <summary>
    /// Used to make requests to the website or various helper classes
    /// </summary>
    public interface IWebsiteRequestHelper
    {
        /// <summary>
        /// Used to get a string response given a uri, username and password
        /// </summary>
        string GetStringResponse(string username, string password, string uri);
        /// <summary>
        /// Used to get a return value from a post request
        /// </summary>
        string PostStringResponse(string username, string password, string uri, string content);
    }
}
