using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Commands.Websites;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// A client used to manage all aspects of a windows azure website
    /// </summary>
    public class WebsiteClient : IWebsiteClient
    {
        /// <summary>
        /// Used to construct a windows azure website client
        /// </summary>
        /// <param name="subscriptionId">the subscription id</param>
        /// <param name="certificate">The management certificate</param>
        /// <param name="siteName">The sitename used to get a context for</param>
        public WebsiteClient(string subscriptionId, X509Certificate2 certificate, string siteName = null)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            Name = siteName;
            if (siteName != null)
                WebsiteProperties = GetWebsiteIfExists();
        }

        /// <summary>
        /// The management certificate
        /// </summary>
        protected X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// The subscription id that the website is in
        /// </summary>
        protected string SubscriptionId { get; set; }

        #region Implementation of IWebsiteClient

        /// <summary>
        /// Returns a list of websites
        /// </summary>
        public List<Website> List()
        {
            // get the lise of webspaces
            var websites = new List<Website>();
            var command = new GetWebsiteListCommand()
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();
            // use this list to derive a list of websites discarding 404 errors
            var list = command.WebsiteRegions;
            foreach (string region in list)
            {
                // the website context command which will return the websites in the various regions
                var context = new WebsiteContextRequestCommand(region)
                                  {
                                      SubscriptionId = SubscriptionId,
                                      Certificate = ManagementCertificate
                                  };
                context.Execute();
                // keep adding the response collections together
                if(context.Websites != null)
                    websites.AddRange(context.Websites);
            }

            return websites;
        }

        /// <summary>
        /// Creates a website given github credentials
        /// </summary>
        /// <param name="website"></param>
        /// <param name="gitDetails">the details of the github repo</param>
        public void CreateFromGithub(Website website, GitDetails gitDetails)
        {
            // create the new website
            Create(website);

            // we have the new website properties
            var githubClient = new GithubClient(new WebsiteRequestHelper())
                                   {
                                       Username = gitDetails.Username,
                                       Password = gitDetails.Password,
                                   };
            var repoList = githubClient.GetRepositories();
            if(!repoList.ContainsKey(gitDetails.RepositoryName))
                throw new FluentManagementException("git repository not found", "WebsiteClient");
            // add the service hook in place 
            bool setServiceHook = githubClient.SetServiceHook(WebsiteProperties.Config.PublishingUsername,
                                        WebsiteProperties.Config.PublishingPassword, website.Name + ".scm.azurewebsites.net",
                                        repoList[gitDetails.RepositoryName], gitDetails.RepositoryName);
            // get all of the auth token values
            githubClient.GetOAuthToken(repoList[gitDetails.RepositoryName], gitDetails.RepositoryName);
            // add the metadata from the service hook
            WebsiteProperties.Config.Metadata.Add("ScmUri", githubClient.ScmUri);
            WebsiteProperties.Config.Metadata.Add("CloneUri", githubClient.CloneUri);
            WebsiteProperties.Config.Metadata.Add("RepoApiUri", githubClient.RepoApiUri);
            WebsiteProperties.Config.Metadata.Add("OAuthToken", githubClient.OAuthToken);
            // update windows azure
            var command = new UpdateWebsiteConfigCommand(WebsiteProperties)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();
            if (setServiceHook) return;
            Delete();
            throw new FluentManagementException(
                "website created but unable to set service hook in github rolled back deployment", "WebsiteClient");
        }

        /// <summary>
        /// As above but using BitBucket instead
        /// </summary>
        public void CreateFromBitBucket(Website website, GitDetails gitDetails)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a default website with nothing deployed
        /// </summary>
        /// <param name="website">the website which will be created</param>
        public void Create(Website website)
        {
            website.WebsiteParameters.CurrentNumberOfWorkers = website.WebsiteParameters.CurrentNumberOfWorkers != 0 ? 
                website.WebsiteParameters.CurrentNumberOfWorkers : 1;
            website.Enabled = true;
            website.State = WebsiteState.Ready;
            website.Webspace = website.Webspace ?? Website.NorthEuropeWebSpace;
            if (!String.IsNullOrEmpty(website.Webspace))
            {
                ValidateWebSpace(website.Webspace);
            }
            // create the website
            var command = new CreateWebsiteCommand(website)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();
            // create the website repo
            var repoCommand = new CreateWebsiteRepositoryCommand(website)
                          {
                              SubscriptionId = SubscriptionId,
                              Certificate = ManagementCertificate
                          };
            repoCommand.Execute();
            WebsiteProperties = website;

            website.WebsiteParameters.NumberOfWorkers = website.WebsiteParameters.NumberOfWorkers == 0
                                                            ? website.WebsiteParameters.CurrentNumberOfWorkers
                                                            : website.WebsiteParameters.NumberOfWorkers;
            WebsiteProperties = GetWebsiteIfExists();

            var command2 = new UpdateWebsiteConfigCommand(WebsiteProperties)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command2.Execute();
            WebsiteProperties = GetWebsiteIfExists();
        }

        /// <summary>
        /// Deletes a website with the current context
        /// </summary>
        public void Delete()
        {
            var site = GetWebsiteIfExists();
            var command = new DeleteWebsiteCommand(site.Webspace, site.Name)
            {
                Certificate = ManagementCertificate,
                SubscriptionId = SubscriptionId
            };
            command.Execute();
        }

        /// <summary>
        /// Gets or sets the number of instances of the website
        /// </summary>
        public int InstanceCount { get; set; }

        /// <summary>
        /// Gets or sets the compute of the site
        /// </summary>
        public ComputeMode ComputeMode { get; set; }

        /// <summary>
        /// Restarts the website from a stopped state
        /// </summary>
        public void Restart()
        {
            var site = GetWebsiteIfExists();
            var command = new WebsiteChangeStateCommand(site, WebsiteState.Running)
            {
                Certificate = ManagementCertificate,
                SubscriptionId = SubscriptionId
            };
            command.Execute();
        }

        /// <summary>
        /// Stops the website if it is currently running
        /// </summary>
        public void Stop()
        {
            var site = GetWebsiteIfExists();
            var command = new WebsiteChangeStateCommand(site, WebsiteState.Stopped)
                              {
                                  Certificate = ManagementCertificate,
                                  SubscriptionId = SubscriptionId
                              };
            command.Execute();
        }

        /// <summary>
        /// The name of the website
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets the website properties
        /// </summary>
        public Website WebsiteProperties { get; set; }

        #endregion

        private Website GetWebsiteIfExists()
        {
            WebsiteParameters websiteParameters = null;
            if(WebsiteProperties != null)
                websiteParameters = WebsiteProperties.WebsiteParameters;

            Name = Name ?? WebsiteProperties.Name;
            if (Name == null)
                throw new FluentManagementException("No name defined for website", "WebsiteClient");
            // get the list of all sites
            var siteList = List();
            // search for the name of the site in the list
            var site = siteList.FirstOrDefault(a => a.Name.ToLowerInvariant() == Name.ToLowerInvariant());
            // make sure that the site exists
            if (site == null)
                throw new FluentManagementException("No site found in this subscription with the name" + Name, "WebsiteClient");
            // get the website configuration
            var command = new GetWebsiteConfigCommand(site)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();
            site.Config = command.Config;
            site.WebsiteParameters = site.WebsiteParameters ?? websiteParameters;
            return WebsiteProperties = site;
        }

        private void ValidateWebSpace(string webSpace)
        {
            if(webSpace != Website.NorthEuropeWebSpace && webSpace != Website.WestEuropeWebSpace)
                throw new FluentManagementException("Ensure you use the correct webspace", "WebsiteClient");
        }
    }
}