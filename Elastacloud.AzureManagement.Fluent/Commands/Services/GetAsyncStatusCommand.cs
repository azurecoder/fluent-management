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
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Gets the async status of an operation given the x-ms-id response - not used for Sql Azure commands 
    /// </summary>
    internal class GetAsyncStatusCommand : ServiceCommand
    {
        /// <summary>
        /// The payload variable to hold the Xml response from the command
        /// </summary>
        internal string XmlResponsePayload { get; set; }

        //https://management.core.windows.net/<subscription-id>/operations/<request-id>
        /// <summary>
        /// Used to get the operation status from the executing command
        /// </summary>
        /// <returns>An OperationStatus value</returns>
        internal OperationStatus GetOperationStatus()
        {
            var document = GetXmlAsyncPayload();
            string services =
                (from item in document.Descendants(QueryManager.BuildDefaultNamespaceXmlEntity("Operation"))
                 select (string) item.Element(QueryManager.BuildDefaultNamespaceXmlEntity("Status"))).FirstOrDefault();
            return (OperationStatus) Enum.Parse(typeof (OperationStatus), services);
        }
        /// <summary>
        /// Used to get the basic payload for the request in XML
        /// </summary>
        private XDocument GetXmlAsyncPayload()
        {
            if (XmlResponsePayload == null)
                throw new ApplicationException("unable to determine response for async operation");
            XDocument document = XDocument.Parse(XmlResponsePayload, LoadOptions.None);
            return document;
        }
        /// <summary>
        /// Used to get the failure text from an async request-response
        /// </summary>
        internal string GetFailureText()
        {
            var document = GetXmlAsyncPayload();
            return document.Element(Namespaces.NsWindowsAzure + "Operation")
                .Element(Namespaces.NsWindowsAzure + "Error")
                .Element(Namespaces.NsWindowsAzure + "Message")
                .Value;
        }

        /// <summary>
        /// Gets the Xml payload from the response and releases the lock 
        /// </summary>
        /// <param name="webResponse">The HttpWebResponse returned from the server request</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                XmlResponsePayload = reader.ReadToEnd().Trim();
            }
            SitAndWait.Set();
        }
    }

    /// <summary>
    /// Determine whether an operation succeeded, failed or is in progress
    /// </summary>
    internal enum OperationStatus
    {
        /// <summary>
        /// Used if an operation fails
        /// </summary>
        Failed,

        /// <summary>
        /// Used if an operation is still in progress
        /// </summary>
        InProgress,

        /// <summary>
        /// Used if an operation has succeeded
        /// </summary>
        Succeeded
    }
}