/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent
{
    /// <summary>
    /// This class is used to build queries and parse XML responses from the Management API 
    /// </summary>
    public static class QueryManager
    {
        /// <summary>
        /// This method makes a callback to an AsyncResponseParser and takes a service management request and certificate and sends a request to 
        /// the management endpoint
        /// </summary>
        public static Task<WebResponse> MakeASyncRequest(ServiceManagementRequest serviceManagementRequest, ServiceManager.AsyncResponseParser parser, ServiceManager.AsyncResponseException error)
        {
            if (ServicePointManager.ServerCertificateValidationCallback == null)
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            ServicePointManager.DefaultConnectionLimit = 40;
            
            HttpWebRequest request = BuildAzureHttpRequest(serviceManagementRequest);

            return Task<WebResponse>.Factory
                .FromAsync(request.BeginGetResponse, request.EndGetResponse, null)
                .ContinueWith<WebResponse>(task =>
                                  {
                                      if (task.Exception != null)
                                      {
                                          if(task.Exception.InnerException.GetType() == typeof(WebException))
                                            error((WebException)task.Exception.InnerException);
                                          return null;
                                      }
                                      var response = (HttpWebResponse)task.Result;
                                      parser(response);
                                      return response;
                                  });
            //IAsyncResult result = request.BeginGetResponse((asyncResult) =>
            //                        {
            //                            try
            //                            {
            //                                var response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            //                                parser(response);
            //                            }
            //                            catch (WebException ex) { error(ex);}
            //                        }, null);
        }

        public static void MakeASyncRequest(ServiceManagementRequest serviceManagementRequest,
                                            ServiceManager.AsyncResponseParser parser)
        {
            MakeASyncRequest(serviceManagementRequest, parser, null);
        }

        /// <summary>
        /// Returns an Xml namespace + entity for the lookup
        /// </summary>
        public static XName BuildDefaultNamespaceXmlEntity(string entityName)
        {
            XNamespace ns = ServiceManager.DefaultWindowsAzureXmlNamespace;
            return ns + entityName;
        }

        #region Helper Methods

        /// <summary>
        /// Builds an Azure request which is then sent to the Management Portal 
        /// </summary>
        private static HttpWebRequest BuildAzureHttpRequest(ServiceManagementRequest serviceManagementRequest)
        {
            string optionalData = serviceManagementRequest.OptionalData != null ? "/" + serviceManagementRequest.OptionalData
                                      : String.Empty;
            string operationId = serviceManagementRequest.OperationId == null ? "" : "/" + serviceManagementRequest.OperationId;
            string serviceType = serviceManagementRequest.ServiceType == null ? ""
                                     : "/" + serviceManagementRequest.ServiceType;
            string requestUriString = String.Format("{0}/{1}", serviceManagementRequest.BaseUri, serviceManagementRequest.SubscriptionId);
            var requestUri = new Uri(requestUriString + serviceType + operationId + optionalData);

            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            if (serviceManagementRequest.Certificate == null && !serviceManagementRequest.RequestWithoutCertificate)
                throw new ApplicationException("unable to send management request without valid certificate");
            if (serviceManagementRequest.Certificate != null)
                request.ClientCertificates.Add(serviceManagementRequest.Certificate);
            foreach (var item in serviceManagementRequest.AdditionalHeaders)
                request.Headers.Add(item.Key, item.Value);
            request.Method = serviceManagementRequest.HttpVerb ?? "GET";
            request.ContentType = serviceManagementRequest.ContentType ?? "application/xml";
            request.ContentLength = 0;

            if (serviceManagementRequest.Body != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(serviceManagementRequest.Body);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            return request;
        }

        #endregion
    }
}