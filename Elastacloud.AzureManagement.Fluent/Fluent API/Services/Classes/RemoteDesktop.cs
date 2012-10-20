/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    /// <summary>
    /// Class used to enable the config for remote desktop
    /// </summary>
    public class RemoteDesktop : ICloudConfig
    {
        private const string CertificateName = "Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption";

        /// <summary>
        /// Used to add the service certificate
        /// </summary>
        private readonly ServiceCertificate _certificate;

        /// <summary>
        /// This class has to be enabled with a service certificate
        /// </summary>
        public RemoteDesktop(ServiceCertificate certificate)
        {
            _certificate = certificate;
        }

        /// <summary>
        /// And or role instance as well 
        /// </summary>
        public RemoteDesktop(ServiceCertificate certificate, string roleName) : this(certificate)
        {
            ((ICloudConfig) this).Rolename = roleName;
        }

        /// <summary>
        /// Contains the username for the remote desktop session
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contains the password for the remote desktop session
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Used primarily to encrypt the password for the Remote Desktop using the pvk of the Service Certificate
        /// </summary>
        public string ExportBase64EncryptedString(string password)
        {
            byte[] byPassword = Encoding.UTF8.GetBytes(password);
            var info = new ContentInfo(byPassword);
            var envelopedCms = new EnvelopedCms(info);
            envelopedCms.Encrypt(new CmsRecipient(_certificate.Certificate));
            return Convert.ToBase64String(envelopedCms.Encode());
            //var provider = (RSACryptoServiceProvider)_certificate.Certificate.PublicKey.Key;
            //byte[] cipherText = provider.Encrypt(byPassword, false);
            //return Convert.ToBase64String(cipherText, Base64FormattingOptions.None);
        }

        #region Implementation of ICloudConfig

        /// <summary>
        /// Used to change the config for the .cscfg and add the params for the 
        /// </summary>
        XDocument ICloudConfig.ChangeConfig(XDocument document)
        {
            // create the plugin entries in the config
            var pluginSettings = new NameValueCollection
                                     {
                                         {Constants.PluginsRemoteAccessEnabled, "true"},
                                         {Constants.PluginsRemoteAccessAccountUsername, Username},
                                         {
                                             Constants.PluginsRemoteAccessAccountEncryptedPassword,
                                             ExportBase64EncryptedString(Password)
                                             },
                                         {
                                             // Build in an ISO 8061 format date and also add 3 months to the expiry of the RD 
                                             Constants.PluginsRemoteAccessAccountExpiration,
                                             DateTime.UtcNow.AddMonths(3).ToString(
                                                 "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")
                                             },
                                         {Constants.PluginsRemoteForwarderEnabled, "true"}
                                     };

            //  Get the role
            XElement role =
                document.Descendants(Namespaces.NsServiceManagement + "Role").FirstOrDefault(
                    a => (string) a.Attribute("name") == ((ICloudConfig) this).Rolename);
            // if configuration settings doesn't exist then create - unliklely 
            XElement configSettings = role.Element(Namespaces.NsServiceManagement + "ConfigurationSettings");
            if (configSettings == null)
                role.Add(configSettings = new XElement(Namespaces.NsServiceManagement + "ConfigurationSettings"));
            // cycle through the name value pairs and each one as a setting
            // TODO: This will blow up the settings exist already so it's best to serialise out this file and determine which exist in advance
            foreach (string pluginSetting in pluginSettings.AllKeys)
            {
                configSettings.Add(new XElement(Namespaces.NsServiceManagement + "Setting",
                                                new XAttribute("name", pluginSetting),
                                                new XAttribute("value", pluginSettings[pluginSetting])));
            }
            // create the certificates node if it doesn't exist already
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
            XElement imports = role.Element(Namespaces.NsServiceDefinition + "Imports");
            if (imports == null)
                role.Add(imports = new XElement(Namespaces.NsServiceDefinition + "Imports"));
            // check to see whether the protocol already exists and raise an exception 
            bool checkImport = imports.Elements(Namespaces.NsServiceDefinition + "Import").Any() &&
                               imports.Elements(Namespaces.NsServiceDefinition + "Import").Count(a =>
                                                                                                     {
                                                                                                         XAttribute
                                                                                                             xAttribute
                                                                                                                 =
                                                                                                                 a.
                                                                                                                     Attribute
                                                                                                                     ("moduleName");
                                                                                                         return
                                                                                                             xAttribute !=
                                                                                                             null &&
                                                                                                             (xAttribute
                                                                                                                  .Value ==
                                                                                                              "RemoteAccess" ||
                                                                                                              xAttribute
                                                                                                                  .Value ==
                                                                                                              "RemoteForwarder");
                                                                                                     }) > 0;
            if (checkImport)
                throw new ApplicationException("Input endpoint already defined for WebRole: " +
                                               ((ICloudConfig) this).Rolename);
            // construct the import tags
            var importAccess = new XElement(Namespaces.NsServiceDefinition + "Import",
                                            new XAttribute("moduleName", "RemoteAccess"));
            var importForwarder = new XElement(Namespaces.NsServiceDefinition + "Import",
                                               new XAttribute("moduleName", "RemoteForwarder"));
            imports.Add(importAccess);
            imports.Add(importForwarder);

            return document;
        }

        string ICloudConfig.Rolename { get; set; }

        #endregion
    }
}