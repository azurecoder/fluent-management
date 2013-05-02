using System;
using System.Net;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    public class WebsiteRequestHelper : IWebsiteRequestHelper
    {
        #region Implementation of IWebsiteRequestHelper

        /// <summary>
        /// Used to get a string response given a uri, username and password
        /// </summary>
        public string GetStringResponse(string username, string password, string uri)
        {
            var client = GetWebClient(username, password);

            if(!uri.StartsWith("https"))
                throw new FluentManagementException("only HTTPS is supported", "WebsiteRequestHelper");

            return client.DownloadString(uri);
        }

        #endregion

        private string GetBasicAuthenticationCredentials(string username, string password)
        {
            var b64EncodedBytes = Encoding.UTF8.GetBytes(username + ":" + password);
            string credentials = Convert.ToBase64String(b64EncodedBytes);

            return "Basic " + credentials;
        }

        private WebClient GetWebClient(string username, string password)
        {
            var client = new WebClient
            {
                Credentials = new NetworkCredential(username, password),
                UseDefaultCredentials = false
            };
            client.Headers.Add("Accept", "application/json");
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            client.Headers.Add("User-Agent", "Mozilla/5.0");
            client.Headers.Add("Authorization", GetBasicAuthenticationCredentials(username, password));

            return client;
        }
    }
}