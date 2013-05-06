using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            Connected = true;

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
        public string SetServiceHook(string publishingUsername, string publishingPassword, string azureRepoName, string accountName, string repoName)
        {
            if(!Connected)
                throw new FluentManagementException("unable to proceed check repository before continuing", "GithubClient");

            // build the hook uri 
            string hook = String.Format("https://{0}:{1}@{2}/deploy", publishingUsername, publishingPassword, azureRepoName);
            // build the hooks post uri 
            string uri = String.Format("https://api.github.com/repos/{0}/{1}/hooks", accountName, repoName);
            string content =
                String.Format("{{\"name\": \"web\",\"active\": true,\"events\": [\"push\"],\"config\": {{\"url\": \"{0}\",\"content_type\": \"json\"}}}}", hook);
            string response = _helper.PostStringResponse(Username, Password, uri, content);
            if (response == null)
                return null;
            return hook;
        }

        /// <summary>
        /// Gets a new oauth token for an application
        /// </summary>
        public void GetOAuthToken(string accountName, string repoName)
        {
            if (!Connected)
                throw new FluentManagementException("unable to proceed check repository before continuing", "GithubClient");

            const string application =
                "{\"scopes\": [\"repo\"],\"note\": \"Fluent Management\",\"note_url\": \"https://fluentmanagement.elastacloud.com\"}";
            // make an auth request using a fluent management application key
            string responseValue = _helper.PostStringResponse(Username, Password, "https://api.github.com/authorizations", application);
            // parse the response value and extract the token to add to github
            var jRepos = JObject.Parse(responseValue);
            // set the oauth token here 
            OAuthToken = jRepos["token"].ToString();
            // get the other details about the repo
            responseValue = _helper.GetStringResponse(Username, Password, String.Format("https://api.github.com/repos/{0}/{1}", accountName, repoName));
            jRepos = JObject.Parse(responseValue);
            // populate the api url
            RepoApiUri = jRepos["url"].ToString();
            // get another uri
            CloneUri = ScmUri = jRepos["html_url"].ToString();
        }

        /// <summary>
        /// The Html_uri of the github repo
        /// </summary>
        public string ScmUri { get; private set; }

        /// <summary>
        /// the clone uri for github
        /// </summary>
        public string CloneUri { get; private set; }

        /// <summary>
        /// Gets the oauth token defining access to the application
        /// </summary>
        public string OAuthToken { get; private set; }

        /// <summary>
        /// Gets the API of the repo http://api.github.com/...
        /// </summary>
        public string RepoApiUri { get; private set; }

        /// <summary>
        /// Tests any service hook
        /// </summary>
        public void TestServiceHook(string hook)
        {
            _helper.ExecuteCommand(hook, 200);
        }

        /// <summary>
        /// Gets the hooks associated with the repo
        /// </summary>
        /// <returns>The ids and uri of the hook</returns>
        public Dictionary<int, string> GetHooks(string repoName)
        {
            var response = _helper.GetStringResponse(Username, Password, repoName);

            var jRepos = JArray.Parse(response);
            return jRepos.ToDictionary(repo => int.Parse(repo["id"].ToString()), repo => repo["uri"].ToString());
        }

        #endregion

       
    }
}
