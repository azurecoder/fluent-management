using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// A client interface to allow connectivity to Github
    /// </summary>
    public interface IGithubClient
    {
        /// <summary>
        /// Used to get all of the repos from Github
        /// </summary>
        Dictionary<string, string> GetRepositories(string uri);
        /// <summary>
        /// The Github username
        /// </summary>
        string Username { get; set; }
        /// <summary>
        /// The Github password 
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// If the login is successful then connected will be true - no operation can occur if connected is false
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// Sets the service hook to allow the azure remote repo to connect and deploy 
        /// </summary>
        bool SetServiceHook(string publishingUsername, string publishingPassword, string azureRepoName);
    }
}
