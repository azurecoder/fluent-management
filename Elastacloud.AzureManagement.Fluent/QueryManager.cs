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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent
{
    public interface IQueryManager
    {
        void SetHttpRequestBuilder(QueryManager.IHttpRequestBuilder builder);
        QueryManager.IHttpRequestBuilder Builder { get; set; }

        /// <summary>
        /// This method makes a callback to an AsyncResponseParser and takes a service management request and certificate and sends a request to 
        /// the management endpoint
        /// </summary>
        Task<WebResponse> MakeASyncRequest(ServiceManagementRequest serviceManagementRequest, ServiceManager.AsyncResponseParser parser, ServiceManager.AsyncResponseException error);

        void MakeASyncRequest(ServiceManagementRequest serviceManagementRequest,
                                              ServiceManager.AsyncResponseParser parser);
    }

    /// <summary>
    /// This class is used to build queries and parse XML responses from the Management API 
    /// </summary>
    public class QueryManager : IQueryManager
    {
        internal QueryManager()
        {
        }

        private IHttpRequestBuilder _builder;

        public void SetHttpRequestBuilder(IHttpRequestBuilder builder)
        {
            Builder = builder;
        }

        public interface IHttpRequestBuilder
        {
            void SetUri(Uri requestUri);
            void AddCertificate(X509Certificate certificate);
            void AddHeader(string key, string value);
            void SetMethod(string method);
            void SetContentType(string contentType);
            void SetBody(string body);
            HttpWebRequest Create();
        }

        public IHttpRequestBuilder Builder
        {
            get
            {
                if (_builder == null)
                    return new BasicHttpRequestBuilder();
                return _builder;
            }
            set { _builder = value; }
        }

        /// <summary>
        /// This method makes a callback to an AsyncResponseParser and takes a service management request and certificate and sends a request to 
        /// the management endpoint
        /// </summary>
        public Task<WebResponse> MakeASyncRequest(ServiceManagementRequest serviceManagementRequest, ServiceManager.AsyncResponseParser parser, ServiceManager.AsyncResponseException error)
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
        }

        public void MakeASyncRequest(ServiceManagementRequest serviceManagementRequest, ServiceManager.AsyncResponseParser parser)
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
        private HttpWebRequest BuildAzureHttpRequest(ServiceManagementRequest serviceManagementRequest)
        {
            string optionalData = serviceManagementRequest.OptionalData != null ? "/" + serviceManagementRequest.OptionalData
                                      : String.Empty;
            string operationId = serviceManagementRequest.OperationId == null ? "" : "/" + serviceManagementRequest.OperationId;
            string serviceType = serviceManagementRequest.ServiceType == null ? ""
                                     : "/" + serviceManagementRequest.ServiceType;
            string requestUriString = String.Format("{0}/{1}", serviceManagementRequest.BaseUri, serviceManagementRequest.SubscriptionId);
            var requestUri = new Uri(requestUriString + serviceType + operationId + optionalData);

            //var request = (HttpWebRequest) WebRequest.Create(requestUri);
            if (serviceManagementRequest.Certificate == null && !serviceManagementRequest.RequestWithoutCertificate)
                throw new ApplicationException("unable to send management request without valid certificate");
            if (serviceManagementRequest.Certificate != null)
                _builder.AddCertificate(serviceManagementRequest.Certificate);//request.ClientCertificates.Add(serviceManagementRequest.Certificate);
            foreach (var item in serviceManagementRequest.AdditionalHeaders)
                _builder.AddHeader(item.Key, item.Value);//request.Headers.Add(item.Key, item.Value);
            _builder.SetMethod(serviceManagementRequest.HttpVerb ?? "GET");//request.Method = serviceManagementRequest.HttpVerb ?? "GET";
            _builder.SetContentType(serviceManagementRequest.ContentType ?? "application/xml");//request.ContentType = serviceManagementRequest.ContentType ?? "application/xml";
            _builder.SetBody(serviceManagementRequest.Body);

            return _builder.Create();
        }

        #endregion
    }
}