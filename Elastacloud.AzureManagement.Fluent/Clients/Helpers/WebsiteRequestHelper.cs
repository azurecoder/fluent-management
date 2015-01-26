/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

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

        /// <summary>
        /// Executes a command request and doesn't wait or process the response but checks the status and throws an exception if not acheived
        /// </summary>
        /// <param name="uri">the uri requested</param>
        /// <param name="status">The Http Status response code expected</param>
        public void ExecuteCommand(string uri, int status)
        {
            HttpClient client = null;

            // do this if the uri should be removed and 
            if (uri.IndexOf(":", 8, System.StringComparison.Ordinal) > 0)
            {
                var posSlash = uri.IndexOf("@", 8, StringComparison.Ordinal);
                string basicAuthCredentials = uri.Substring(8, posSlash - 8);
                var parts = basicAuthCredentials.Split(':');
                client = GetWebClient(parts[0], parts[1]);
                uri = uri.Remove(8, (basicAuthCredentials + "/").Length);
            }
            else
            {
                client = GetWebClient();
            }
            var responseCode = (int)client.GetAsync(uri).Result.StatusCode;
            if(responseCode != status)
                throw new FluentManagementException("incorrect response code returned code is: " + responseCode, "WebsiteRequestHelper");
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
            var client = GetWebClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetBasicAuthenticationCredentials(username, password));

            return client;
        }

        private HttpClient GetWebClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

            return client;
        }
        
    }
}