/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Certificates
{
    /// <summary>
    /// Used to add a service certificate to a particular hosted service so that it can be used in remote desktop or SSL
    /// </summary>
    internal class AddServiceCertificateCommand : ServiceCommand
    {
        /// <summary>
        /// Sets the REST command parameters and takes a byte array converting certificate data to base64, the password has to match the 
        /// password that was given when the certificate was first created
        /// </summary>
        internal AddServiceCertificateCommand(byte[] certificateData, string pfxPassword, string hostedServiceName)
        {
            Base64CertificateData = Convert.ToBase64String(certificateData);
            PfxPassword = pfxPassword;
            HostedServiceName = hostedServiceName;
            ServiceType = "services";
            OperationId = "hostedservices";
            HttpCommand = hostedServiceName + "/certificates";
            HttpVerb = HttpVerbPost;
        }

        /// <summary>
        /// The private key password
        /// </summary>
        internal string PfxPassword { get; set; }

        /// <summary>
        /// The certificate data encoded into Base64
        /// </summary>
        internal string Base64CertificateData { get; set; }

        /// <summary>
        /// The name of the hosted service to apply the operation on
        /// </summary>
        internal string HostedServiceName { get; set; }

        //https://management.core.windows.net/<subscription-id>/certificates/
        /// <summary>
        /// The creation of the XML payload necessary to make the request
        /// </summary>
        protected override string CreatePayload()
        {
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "CertificateFile",
                             new XElement(ns + "Data", Base64CertificateData),
                             new XElement(ns + "CertificateFormat", "pfx"),
                             new XElement(ns + "Password", PfxPassword)));
            return doc.ToStringFullXmlDeclaration();
        }
    }
}