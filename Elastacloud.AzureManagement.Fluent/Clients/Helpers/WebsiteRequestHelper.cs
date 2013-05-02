using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            return client.GetStringAsync(uri).Result;
        }

        /// <summary>
        /// Used to get a return value from a post request
        /// </summary>
        public string PostStringResponse(string username, string password, string uri, string content)
        {
            var client = GetWebClient(username, password);

            if (!uri.StartsWith("https"))
                throw new FluentManagementException("only HTTPS is supported", "WebsiteRequestHelper");

            return client.PostAsync(uri, new StringContent(content)).Result.Content.ReadAsStringAsync().Result;
        }

        #endregion

        private string GetBasicAuthenticationCredentials(string username, string password)
        {
            var b64EncodedBytes = Encoding.UTF8.GetBytes(username + ":" + password);
            string credentials = Convert.ToBase64String(b64EncodedBytes);

            return credentials;
        }

        private HttpClient GetWebClient(string username, string password)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetBasicAuthenticationCredentials(username, password));

            return client;
        }
    }
}