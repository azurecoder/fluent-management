/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary>
    /// A simple type to contain a service management request
    /// </summary>
    // TODO: Consolidate the certificate file path into this structure
    public struct ServiceManagementRequest
    {
        public Dictionary<string, string> AdditionalHeaders { get; set; }

        /// <summary>
        /// The base uri used to invoke the Azure RESTful service
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// The subscription id used to get the service information
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// The type of service being requested e.g. hostedservice
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// The operation id being used e.g. getservice
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// Can be GET, PUT, POST, DELETE
        /// </summary>
        public string HttpVerb { get; set; }

        /// <summary>
        /// By default the content type is set to application/xml
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// A XML document containing the details of the POST or PUT
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Value contains a bypass for the certificate check 
        /// </summary>
        public bool RequestWithoutCertificate { get; set; }

        /// <summary>
        /// Add a certificate to the service management request
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Optional data for the request e.g. name of a hosted service 
        /// </summary>
        public string OptionalData { get; set; }
    }
}