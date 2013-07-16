using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Websites;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
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
        /// <summary>
        /// Contains a running total of all of the region counts related to that subscription
        /// </summary>
        private Dictionary<string, int> regionCounts = new Dictionary<string, int>(); 

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
            foreach (WebspaceProperties region in list)
            {
                // the website context command which will return the websites in the various regions
                var context = new WebsiteContextRequestCommand(region.Name)
                                  {
                                      SubscriptionId = SubscriptionId,
                                      Certificate = ManagementCertificate
                                  };
                context.Execute();
                // keep adding the response collections together
                if(context.Websites != null)
                    websites.AddRange(context.Websites);
                // add the number of instances to the running collection for the webspace
                regionCounts.Add(region.Name, region.InstanceCount);
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
            string setServiceHook = githubClient.SetServiceHook(WebsiteProperties.Config.PublishingUsername,
                                        WebsiteProperties.Config.PublishingPassword, website.Name + ".scm.azurewebsites.net",
                                        repoList[gitDetails.RepositoryName], gitDetails.RepositoryName);
            
            // get all of the auth token values
            githubClient.GetOAuthToken(repoList[gitDetails.RepositoryName], gitDetails.RepositoryName);
            // add the metadata from the service hook
            WebsiteProperties.Config.Metadata.Add("ScmUri", githubClient.ScmUri);
            WebsiteProperties.Config.Metadata.Add("CloneUri", githubClient.CloneUri);
            WebsiteProperties.Config.Metadata.Add("RepoApiUri", githubClient.RepoApiUri);
            WebsiteProperties.Config.Metadata.Add("OAuthToken", githubClient.OAuthToken);
            WebsiteProperties.Config.ScmType = ScmType.GitHub;
            // update windows azure
            Update();
            // call the service hook
            //githubClient.TestServiceHook(setServiceHook);
            if (!String.IsNullOrEmpty(setServiceHook)) return;
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
        /// <param name="scm"></param>
        public void Create(Website website, ScmType scm = ScmType.LocalGit)
        {
            website.WebsiteParameters.CurrentNumberOfWorkers = website.WebsiteParameters.CurrentNumberOfWorkers != 0 ? 
                website.WebsiteParameters.CurrentNumberOfWorkers : 1;
           
            website.Enabled = true;
            website.State = WebsiteState.Ready;
            website.Webspace = website.Webspace ?? WebspaceLocationConstants.NorthEuropeWebSpace;
            if (!String.IsNullOrEmpty(website.Webspace))
            {
                ValidateWebSpace(website.Webspace);
            }
            SetScalePotential(website);
            // check to see whether this is dedicate first
           // if((website.ServerFarm != null && website.ServerFarm.InstanceCount))


            // create the website
            var command = new CreateWebsiteCommand(website)
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();
            // create the website repo
            // do this only if local git is selected
            var repoCommand = new CreateWebsiteRepositoryCommand(website)
                                    {
                                        SubscriptionId = SubscriptionId,
                                        Certificate = ManagementCertificate
                                    };
            repoCommand.Execute();
            WebsiteProperties = website;

            // TODO: fix this value in code!!!!
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
        public int InstanceCount
        {
            get
            {
                if (WebsiteProperties != null && WebsiteProperties.Config != null)
                    return WebsiteProperties.Config.NumberOfWorkers;
                throw new FluentManagementException("unable to determine number of workers - ensure you have a website context", "WebsiteClient");
            }
            set
            {
                if (WebsiteProperties != null && WebsiteProperties.Config != null)
                    WebsiteProperties.Config.NumberOfWorkers = value;
            }
        }

        /// <summary>
        /// Gets or sets the compute of the site
        /// </summary>
        public ComputeMode ComputeMode 
        { 
            get
            {
                if (WebsiteProperties != null)
                    return WebsiteProperties.ComputeMode;
                throw new FluentManagementException("unable to determine compute mode - ensure you have a website context", "WebsiteClient");
            }
            set
            {
                if (WebsiteProperties != null)
                    WebsiteProperties.ComputeMode = value;
            }
        }

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
        /// Updates the config with the current compute config
        /// </summary>
        public void Update()
        {
            var updateCommand = new UpdateWebsiteConfigCommand(WebsiteProperties)
                                    {
                                        SubscriptionId = SubscriptionId,
                                        Certificate = ManagementCertificate
                                    };
            updateCommand.Execute();
        }

        /// <summary>
        /// Gets the website metrics in the requested interval backwards from datetime.now
        /// </summary>
        public List<WebsiteMetric> GetWebsiteMetricsPerInterval(TimeSpan span)
        {
            // use the timespan here with the query to get back the metric values for the interval
            var start = DateTime.UtcNow - span;
            var end = DateTime.UtcNow;

            var command = new GetWebsiteMetricsCommand(WebsiteProperties, start, end)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            command.Execute();
            return command.WebsiteMetrics;
        }

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
            if (webSpace != WebspaceLocationConstants.NorthEuropeWebSpace && webSpace != WebspaceLocationConstants.WestEuropeWebSpace &&
                webSpace != WebspaceLocationConstants.EastUSWebSpace)
                throw new FluentManagementException("Ensure you use the correct webspace", "WebsiteClient");
        }

        private void SetScalePotential(Website website)
        {
            // if the type hasn't been defined then set the default to free
            switch (website.ComputeMode)
            {
                case ComputeMode.Free:
                    website.ComputeMode = ComputeMode.Shared;
                    website.Mode = SiteMode.Limited;
                    break;
                case ComputeMode.Shared:
                    website.ComputeMode = ComputeMode.Shared;
                    website.Mode = SiteMode.Basic;
                    break;
                case ComputeMode.Dedicated:
                    {
                        website.ComputeMode = ComputeMode.Dedicated;
                        website.Mode = SiteMode.Basic;
                        // TODO: Create the default server farm here and add to config
                        try
                        {
                            // check to see whether this has been set or not
                            if (website.ServerFarm == null)
                            {
                                website.ServerFarm = new ServerFarm() {Name = "DefaultServerFarm"};    
                            }
                            // then use this to get or create the server farm
                            var command = new GetServerFarmCommand(website)
                                {
                                    SubscriptionId = SubscriptionId,
                                    Certificate = ManagementCertificate
                                };
                            command.Execute();
                            WebsiteProperties = command.Website;
                        }
                        catch (Exception)
                        {
                            // if we get the exception here then this will most likely be a 404 - for the time being treat it as such ...
                            try
                            {
                                var command = new CreateWebsiteServerFarmCommand(website)
                                {
                                    SubscriptionId = SubscriptionId,
                                    Certificate = ManagementCertificate
                                };
                                command.Execute();
                            }
                            catch (Exception)
                            {
                                // if this fails we want to halt
                                throw new FluentManagementException(
                                    "Subscription does not support a dedicated server farm", "WebsiteClient");
                            }
                            
                        }
                        break;
                    }
            }

        }
    }
}