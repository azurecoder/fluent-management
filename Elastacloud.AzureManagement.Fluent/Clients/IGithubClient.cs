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
        string SetServiceHook(string publishingUsername, string publishingPassword, string azureRepoName, string accountName, string repoName);
        /// <summary>
        /// Gets a new oauth token for an application
        /// </summary>
        void GetOAuthToken(string accountName, string repoName);
        /// <summary>
        /// The Html_uri of the github repo
        /// </summary>
        string ScmUri { get; }
        /// <summary>
        /// the clone uri for github
        /// </summary>
        string CloneUri { get; }
        /// <summary>
        /// Gets the oauth token defining access to the application
        /// </summary>
        string OAuthToken { get; }
        /// <summary>
        /// Gets the API of the repo http://api.github.com/...
        /// </summary>
        string RepoApiUri { get; }
        /// <summary>
        /// Tests any service hook
        /// </summary>
        void TestServiceHook(string hook);
        /// <summary>
        /// Gets the hooks associated with the repo
        /// </summary>
        /// <returns>The ids and uri of the hook</returns>
        Dictionary<int, string> GetHooks(string repoName);
    }
}
