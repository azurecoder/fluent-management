using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Websites;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class WebsiteClient :IWebsiteClient
    {
        public WebsiteClient(string subscriptionId, X509Certificate2 certificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
        }

        protected X509Certificate2 ManagementCertificate { get; set; }
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
                Console.WriteLine(region);
                var context = new WebsiteContextRequestCommand(region)
                                  {
                                      SubscriptionId = SubscriptionId,
                                      Certificate = ManagementCertificate
                                  };
                context.Execute();
                if(context.Websites != null)
                    websites.AddRange(context.Websites);
            }

            return websites;
        }

        #endregion
    }
}