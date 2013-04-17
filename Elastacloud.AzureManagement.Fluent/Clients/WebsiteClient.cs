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
            var command = new GetWebsiteListCommand()
                              {
                                  SubscriptionId = SubscriptionId,
                                  Certificate = ManagementCertificate
                              };
            command.Execute();

            var list = command.WebsiteRegions;
            foreach (string website in list)
            {
                Console.WriteLine(website);
                var context = new WebsiteContextRequestCommand(website)
                                  {
                                      SubscriptionId = SubscriptionId,
                                      Certificate = ManagementCertificate
                                  };
                context.Execute();
            }

            return null;
        }

        #endregion
    }
}