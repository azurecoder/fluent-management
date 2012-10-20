/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Linq;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    public class SslEnablement : ICloudConfig
    {
        public const string CertificateName = "Microsoft.WindowsAzure.Certificate.SSLCert";
        private readonly ServiceCertificate _certificate;

        public SslEnablement(ServiceCertificate certificate)
        {
            _certificate = certificate;
        }

        public SslEnablement(ServiceCertificate certificate, string roleName) : this(certificate)
        {
            ((ICloudConfig) this).Rolename = roleName;
        }

        #region Implementation of ICloudConfig

        XDocument ICloudConfig.ChangeConfig(XDocument document)
        {
            XElement role = document.Descendants(Namespaces.NsServiceManagement + "Role")
                .FirstOrDefault(a => (string) a.Attribute("name") == ((ICloudConfig) this).Rolename);

            XElement cert = role.Element(Namespaces.NsServiceManagement + "Certificates");
            if (cert == null)
                role.Add(cert = new XElement(Namespaces.NsServiceManagement + "Certificates"));

            // check to see if there is a Service Cert and if so then add it via thumbprint to the doc
            var serviceCertificate = new XElement(Namespaces.NsServiceManagement + "Certificate",
                                                  new XAttribute("name", CertificateName),
                                                  new XAttribute("thumbprint", _certificate.Certificate.Thumbprint),
                                                  new XAttribute("thumbprintAlgorithm", "sha1"));
            cert.Add(serviceCertificate);

            return document;
        }

        XDocument ICloudConfig.ChangeDefinition(XDocument document)
        {
            if (((ICloudConfig) this).Rolename == null)
                throw new ApplicationException("Rolename must be defined");
            XElement role = document.Descendants(Namespaces.NsServiceDefinition + "WebRole")
                .FirstOrDefault(a => (string) a.Attribute("name") == ((ICloudConfig) this).Rolename);

            // build input endpoint 
            XElement endpoints = role.Element(Namespaces.NsServiceDefinition + "Endpoints");
            if (endpoints == null)
                role.Add(endpoints = new XElement(Namespaces.NsServiceDefinition + "Endpoints"));
            // check to see whether the protocol already exists and raise an exception 
            bool checkEndpoint = endpoints.Elements(Namespaces.NsServiceDefinition + "InputEndpoint").Any() &&
                                 endpoints.Elements(Namespaces.NsServiceDefinition + "InputEndpoint").Count(a =>
                                                                                                                {
                                                                                                                    XAttribute
                                                                                                                        xAttribute
                                                                                                                            =
                                                                                                                            a
                                                                                                                                .
                                                                                                                                Attribute
                                                                                                                                ("protocol");
                                                                                                                    return
                                                                                                                        xAttribute !=
                                                                                                                        null &&
                                                                                                                        xAttribute
                                                                                                                            .
                                                                                                                            Value ==
                                                                                                                        "https";
                                                                                                                }) > 0;
            if (checkEndpoint)
                throw new ApplicationException("Input endpoint already defined for WebRole: " +
                                               ((ICloudConfig) this).Rolename);
            var inputEndpoint = new XElement(Namespaces.NsServiceDefinition + "InputEndpoint",
                                             new XAttribute("name", "EndpointSSL"),
                                             new XAttribute("protocol", "https"),
                                             new XAttribute("port", "443"),
                                             new XAttribute("certificate", CertificateName));
            endpoints.Add(inputEndpoint);
            // now we want to add the bindings
            XElement webSite = role.Descendants(Namespaces.NsServiceDefinition + "Site").FirstOrDefault(a =>
                                                                                                            {
                                                                                                                XAttribute
                                                                                                                    attribute
                                                                                                                        =
                                                                                                                        a
                                                                                                                            .
                                                                                                                            Attribute
                                                                                                                            ("name");
                                                                                                                return
                                                                                                                    attribute !=
                                                                                                                    null &&
                                                                                                                    attribute
                                                                                                                        .
                                                                                                                        Value ==
                                                                                                                    "Web";
                                                                                                            });
            XElement bindings = webSite.Elements(Namespaces.NsServiceDefinition + "Bindings").FirstOrDefault();
            if (bindings == null)
                webSite.Add(bindings = new XElement(Namespaces.NsServiceDefinition + "Bindings"));
            var binding = new XElement(Namespaces.NsServiceDefinition + "Binding", new XAttribute("name", "EndpointSSL"),
                                       new XAttribute("endpointName", "EndpointSSL"));
            bindings.Add(binding);

            // TODO: Remove this and consolidate this code into a single method AOP!
            // build input endpoint 
            XElement certificates = role.Element(Namespaces.NsServiceDefinition + "Certificates");
            if (certificates == null)
                role.Add(certificates = new XElement(Namespaces.NsServiceDefinition + "Certificates"));
            var certificate = new XElement(Namespaces.NsServiceDefinition + "Certificate",
                                           new XAttribute("name", CertificateName),
                                           new XAttribute("storeLocation", "LocalMachine"),
                                           new XAttribute("storeName", "My"));
            certificates.Add(certificate);
            return document;
        }

        string ICloudConfig.Rolename { get; set; }

        #endregion
    }
}