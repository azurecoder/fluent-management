/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Abstract base class used to encapsulate a command to a hosted service
    /// </summary>
    internal class ServiceCommand : ICommand
    {
        #region private members

        private string _httpVerb = "POST";
        private static IQueryManager _queryManager;

        #endregion

        #region constants

        internal const string HttpVerbPut = "PUT";
        internal const string HttpVerbDelete = "DELETE";
        internal const string HttpVerbPost = "POST";
        internal const string HttpVerbGet = "GET";
        internal const string VersionHeader = "2011-08-18";

        #endregion

        #region abstract and virtual methods

        /// <summary>
        /// Creates an empty Xml payload since the service command itself does not expect a response - overriden in a derived class which does
        /// </summary>
        protected virtual string CreateXmlPayload()
        {
            return null;
        }

        /// <summary>
        /// Initially used via a response callback for commands which expect a async response 
        /// </summary>
        /// <param name="webResponse">the HttpWebResponse that will be sent back to the user from the request</param>
        protected virtual void ResponseCallback(HttpWebResponse webResponse)
        {
            //Track and throw up the X-ms request id (x-ms-request-id)
            MsftAsyncResponseId = webResponse.GetResponseHeader("x-ms-request-id");
            ElastaLogger.MessageLogger.Trace("Hosted Service Response Id: {0}", MsftAsyncResponseId);
            for (;;)
            {
                var asyncCommand = new GetAsyncStatusCommand
                                       {
                                           HttpVerb = HttpVerbGet,
                                           SubscriptionId = SubscriptionId,
                                           OperationId = MsftAsyncResponseId,
                                           ServiceType = "operations",
                                           Certificate = Certificate
                                       };
                asyncCommand.Execute();
                Thread.Sleep(1000);
                OperationStatus status = asyncCommand.GetOperationStatus();
                switch (status)
                {
                    case OperationStatus.InProgress:
                        break;
                    case OperationStatus.Failed:
                        ElastaLogger.MessageLogger.Error("Hosted Service Response Id: {0}", MsftAsyncResponseId);
                        SitAndWait.Set();
                        return;
                    case OperationStatus.Succeeded:
                        ElastaLogger.MessageLogger.Trace("Hosted Service Response Id: {0}", MsftAsyncResponseId);
                        SitAndWait.Set();
                        return;
                }
            }
        }

        /// <summary>
        /// The error callback exception that will be attached to if the response presents a failure of some sort -
        /// to be routed to a notification interface of a certain type
        /// </summary>
        /// <param name="exception">The web exception as it stands</param>
        protected virtual void ErrorResponseCallback(WebException exception)
        {
            // do a simple trace here
            string message = string.Format("Error with status code: {0} and type: {1}",
                                           ((int) exception.Status).ToString(CultureInfo.InvariantCulture),
                                           exception.Status.ToString());
            ElastaLogger.MessageLogger.TraceException(message, exception);
            // add NLog support for the exception
            ElastaLogger.MessageLogger.Error(exception.Message);
            // TODO: Place and error router here 

            // if we have an error it's probably best to release this
            SitAndWait.Set();
            // rethrow this otherwise we'll lose this 
            throw exception;
        }

        #endregion

        #region internal Properties

        internal string Name { get; set; }
        internal string Description { get; set; }
        internal string Location { get; set; }

        internal string HttpVerb
        {
            get { return _httpVerb; }
            set { _httpVerb = value; }
        }

        internal string HttpCommand { get; set; }
        internal string ServiceType { get; set; }
        internal string OperationId { get; set; }
        internal string SubscriptionId { get; set; }
        internal X509Certificate2 Certificate { get; set; }
        internal string MsftAsyncResponseId { get; set; }
        internal ManualResetEvent SitAndWait { get; set; }
        internal Dictionary<string, string> AdditionalHeaders { get; set; }
        internal string BaseRequestUri { get; set; }

        /// <summary>
        /// Used to set the content-type header - by default should be application/xml
        /// </summary>
        internal string ContentType { get; set; }

        /// <summary>
        /// Sets the flag as to whether the certificate is used or not
        /// </summary>
        internal bool? UseCertificate { get; set; }

        #endregion

        #region Constants

        internal const string BaseUri = "https://management.core.windows.net";

        #endregion

        /// <summary>
        /// Do not remove - only set for Unit testing to inject a mock without requiring ctor changes to commands
        /// </summary>
        internal static IQueryManager CurrentQueryManager
        {
            get
            {
                if (_queryManager==null)
                    return new QueryManager();

                return _queryManager;
            }
            set { _queryManager = value; }
        }

        protected ServiceCommand()
        {
            SitAndWait = new ManualResetEvent(false);
            BaseRequestUri = BaseUri;
            AdditionalHeaders = new Dictionary<string, string> {{"x-ms-version", "2011-10-01"}};
        }

        #region ICommand Members

        /// <summary>
        /// Executes the request and waits for a response from the Service Management API
        /// Control is delegated back to the calling class when the reponse comes back which releases the WaitHandle
        /// </summary>
        public virtual void Execute()
        {
            var serviceManagementRequest = new ServiceManagementRequest
                                               {
                                                   BaseUri = BaseRequestUri,
                                                   HttpVerb = HttpVerb,
                                                   OptionalData = HttpCommand,
                                                   ServiceType = ServiceType,
                                                   OperationId = OperationId,
                                                   SubscriptionId = SubscriptionId,
                                                   Body = CreateXmlPayload(),
                                                   Certificate = Certificate,
                                                   AdditionalHeaders = AdditionalHeaders,
                                                   ContentType = ContentType,
                                                   RequestWithoutCertificate =
                                                       !(UseCertificate.HasValue && UseCertificate.Value)
                                               };
            _queryManager.MakeASyncRequest(serviceManagementRequest, ResponseCallback, ErrorResponseCallback);
            // wait for up to 30 minutes - if a deployment takes longer than that ... it's probably HPC!
            SitAndWait.WaitOne(200000);
        }

        #endregion

        /// <summary>
        /// Used to return the routable value of the command to the correct notification interface 
        /// </summary>
        /// <returns>A string value containing the MS command name if it exists in the map</returns>
        public override string ToString()
        {
            var map = new CommandNameMap();
            return map.GetCommandName(this);
        }

        /// <summary>
        /// Used to get and instance of the BaseParser that will parse the Xml response from the Fabric
        /// </summary>
        /// <param name="response">the HttpWebResponse that is returned</param>
        /// <param name="root">the root element neededs</param>
        /// <param name="baseParser">used to parse the response coming back</param>
        /// <returns>A dynamic type based on the expected return</returns>
        public static dynamic Parse(HttpWebResponse response, string root, BaseParser baseParser = null)
        {
            BaseParser parser = BaseParser.GetInstance(response, root, baseParser);
            parser.Parse();
            return parser.CommandResponse;
        }

        /// <summary>
        /// Gets the Uri as per the request to Windows Azure
        /// </summary>
        public Uri RequestUri
        {
            get
            {
                string optionalData = HttpCommand != null ? "/" + HttpCommand : String.Empty;
                string operationId = OperationId == null ? "" : "/" + OperationId;
                string serviceType = ServiceType == null ? "" : "/" + ServiceType;
                string requestUriString = String.Format("{0}/{1}", BaseUri, SubscriptionId);
                return new Uri(requestUriString + serviceType + operationId + optionalData);
            }
        }
    }
}