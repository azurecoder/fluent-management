using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        /// <param name="repositoryName">the name of the repository to create</param>
        /// <param name="username">The github username used to login</param>
        /// <param name="password">The github password to login</param>
        public void CreateFromGithub(string repositoryName, string username, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a default website with nothing deployed
        /// </summary>
        /// <param name="hostname">The hostname of the website</param>
        /// <param name="location">the location where the website will be deployed to</param>
        public void Create(string hostname, string location = LocationConstants.NorthEurope)
        {
            throw new NotImplementedException();
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
            if (Name == null)
                throw new FluentManagementException("No name defined for website", "WebsiteClient");
            // get the list of all sites
            var siteList = List();
            // search for the name of the site in the list
            var site = siteList.FirstOrDefault(a => a.Name.ToLowerInvariant() == Name.ToLowerInvariant());
            // make sure that the site exists
            if (site == null)
                throw new FluentManagementException("No site found in this subscription with the name" + Name, "WebsiteCliet");
            return WebsiteProperties = site;
        }
    }
}