using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Newtonsoft.Json.Linq;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Used to connect to the Github repos
    /// </summary>
    public class GithubClient : IGithubClient
    {
        private readonly IWebsiteRequestHelper _helper;

        public GithubClient(IWebsiteRequestHelper helper)
        {
            _helper = helper;
        }

        #region Implementation of IGithubClient

        /// <summary>
        /// Used to get all of the repos from Github
        /// </summary>
        public Dictionary<string, string> GetRepositories(string uri = "https://api.github.com/user/repos")
        {
            if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
                throw new FluentManagementException("Invalid username or password for Github", "GithubClient");

            string repos = _helper.GetStringResponse(Username, Password, uri);
            var dictionary = new Dictionary<string, string>();
            var jRepos = JArray.Parse(repos);
            foreach (var parts in jRepos.Select(jRepo => jRepo["full_name"].ToString()).Select(name => name.Split('/')))
            {
                if (parts.Length != 2)
                    throw new FluentManagementException("unable to parse JSON response", "GithubClient");
                dictionary.Add(parts[1], parts[0]);
            }

            return dictionary;
        }

        

        /// <summary>
        /// The Github username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The Github password 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// If the login is successful then connected will be true - no operation can occur if connected is false
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Sets the service hook to allow the azure remote repo to connect and deploy 
        /// </summary>
        public bool SetServiceHook(string publishingUsername, string publishingPassword, string azureRepoName)
        {
            throw new NotImplementedException();
        }

        #endregion

       
    }
}
