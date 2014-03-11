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
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Subscriptions;
using Elastacloud.AzureManagement.Fluent.Properties;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings
{
    /// <summary>
    /// Enum illustrates two possible file types publishsettings and certificate
    /// </summary>
    public enum FileType
    {
        PublishSettings,
        Certificate
    }

    /// <summary>
    /// Used to get certificates from the store or from .publishsettings
    /// </summary>
    public class PublishSettingsExtractor
    {
        private List<Subscription> _subscriptions = new List<Subscription>();
        public double SchemaVersion { get; set; }
        /// <summary>
        /// The filepath to the .publishsettings file
        /// </summary>
        private readonly string _publishSettingsFile = null;
        /// <summary>
        /// The XDocument for the publishsettings xml
        /// </summary>
        private readonly XDocument _publishSettingsFileXml = null;

        /// <summary>
        /// Used to construct a publishsetting extractor class
        /// </summary>
        /// <param name="filePath">The filepath to the .publishsettings file</param>
        /// <param name="xmlContent">Used to define whether the string is xml or a file</param>
        public PublishSettingsExtractor(string filePath, bool xmlContent = false)
        {
            _publishSettingsFileXml = xmlContent ? XDocument.Parse(filePath) : XDocument.Load(filePath);
            Parse();
        }

        /// <summary>
        /// The indexer takes a subscription id string and returns a Subscription 
        /// </summary>
        public Subscription this[string subscriptionId]
        {
            get
            {
                if (_subscriptions.Count == 0)
                    GetSubscriptions();
                return _subscriptions.FirstOrDefault(subscription => subscription.Id == subscriptionId);
            }
        }

        /// <summary>
        /// Returns a PublishSettingsExtractor given a valid path 
        /// </summary>
        public static PublishSettingsExtractor GetFromFile(string file)
        {
            return new PublishSettingsExtractor(file);
        }

        /// <summary>
        /// Returns a PublishSettingsExtractor given a valid xml document
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static PublishSettingsExtractor GetFromXml(string xml)
        {
            return new PublishSettingsExtractor(xml, true);
        }

        /// <summary>
        /// Used to parse a .publishsettings file to determine the schema 
        /// </summary>
        private void Parse()
        {
            var schemaAttrs = _publishSettingsFileXml.Descendants("PublishProfile").FirstOrDefault().Attributes("SchemaVersion");
            var containsSchema = schemaAttrs.Any();
            SchemaVersion = containsSchema ? (int)double.Parse(schemaAttrs.FirstOrDefault().Value, CultureInfo.InvariantCulture) : 1D;

            _subscriptions = _publishSettingsFileXml.Descendants("Subscription").Select(a => new Subscription()
                                                                    { 
                                                        Id = a.Attribute("Id").Value,
                                                        Name = a.Attribute("Name").Value,
                                                        ManagementUrl = "https://management.core.windows.net/",
                                                        ManagementCertificate = GetCertificateFromFile(a.Attribute("ManagementCertificate").Value)
                                                                    }).ToList();
        }

        /// <summary>
        /// Gets a list of subscriptions in a .publishsettings file
        /// </summary>
        /// <returns>a list of subscriptions</returns>
        public List<Subscription> GetSubscriptions()
        {
            return _subscriptions;
        }

        /// <summary>
        /// Gets the certificate thumbprint value from the publishsettings xml
        /// </summary>
        /// <returns>A valid SHA1 thumbprint of the certificate</returns>
        public string GetCertificateThumbprint()
        {
            var cert = GetCertificateFromFile();
            return cert.Thumbprint;
        }

        /// <summary>
        /// Will extract a .publishsettings file certificate and then export as a .cer file
        /// </summary>
        public void Extract(string fileNameOut)
        {
            X509Certificate2 cert = GetCertificateFromFile();
            byte[] content = cert.Export(X509ContentType.Cert);
            using (FileStream stream = File.Create(fileNameOut))
            {
                stream.Write(content, 0, content.Count());
            }
        }

        /// <summary>
        /// Checks through the CurrentUser and LocalMachine Personal and AuthRoot stores
        /// </summary>
        public static X509Certificate2 FromStore(string thumbprint, StoreLocation location = StoreLocation.CurrentUser)
        {
            X509Certificate2 certificate2 = null;
            certificate2 = ((FromStore(thumbprint, StoreName.My, location) ?? FromStore(thumbprint, StoreName.AuthRoot, location)));
            if (certificate2 == null)
                throw new ApplicationException("unable to find given certificate");
            return certificate2;
        }

        /// <summary>
        /// Takes an xml document and adds to a store 
        /// </summary>
        /// <returns>An X509Certificate2 certificate object</returns>
        public X509Certificate2 AddPublishSettingsToPersonalMachineStore(StoreLocation location = StoreLocation.CurrentUser)
        {
            return AddPublishSettingsToPersonalMachineStore(_publishSettingsFileXml.ToStringFullXmlDeclaration());
        }
        /// <summary>
        /// Publishsettings file added to the machine store with the subcription id 
        /// </summary>
        public List<X509Certificate2> AddAllPublishSettingsCertificatesToPersonalMachineStore(string subscriptionId = null)
        {
            var certificates = new List<X509Certificate2>();
            foreach (var subscription in GetSubscriptions().Where(subscription => String.IsNullOrEmpty(subscriptionId) || subscription.Id == subscriptionId))
            {
                try
                {
                    FromStore(subscription.ManagementCertificate.Thumbprint);
                }
                catch (ApplicationException)
                {
                    // if we reach here then it's not in the store and we want to put there!!
                    var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(subscription.ManagementCertificate);
                    store.Close();
                }
                certificates.Add(subscription.ManagementCertificate);
            }
            return certificates;
        }

        /// <summary>
        /// Adds a certificate to the local machine personal store 
        /// </summary>
        public static X509Certificate2 AddPublishSettingsToPersonalMachineStore(string xml, StoreLocation location = StoreLocation.CurrentUser)
        {
            X509Certificate2 certificate = GetCertificateFromXml(xml);
            try
            {
                FromStore(certificate.Thumbprint);
            }
            catch (ApplicationException)
            {
                // if we reach here then it's not in the store and we want to put there!!
                var store = new X509Store(StoreName.My, location);
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                store.Close();
            }

            return certificate;
        }

        /// <summary>
        /// Used to check whether the certificate is in the underlying storename and location
        /// </summary>
        private static X509Certificate2 FromStore(string thumbprint, StoreName storeName, StoreLocation location = StoreLocation.CurrentUser)
        {
            var store = new X509Store(storeName, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
            }
            catch (Exception e)
            {
                if (e is CryptographicException)
                {
                    Console.WriteLine(Resources.PublishSettingsExtractor_FromStore_Error__The_store_is_unreadable_);
                }
                else if (e is SecurityException)
                {
                    Console.WriteLine(
                        Resources.PublishSettingsExtractor_FromStore_Error__You_don_t_have_the_required_permission_);
                }
                else if (e is ArgumentException)
                {
                    Console.WriteLine(Resources.PublishSettingsExtractor_FromStore_Error__Invalid_values_in_the_store_);
                }
                else
                {
                    throw;
                }
            }

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            store.Close();

            if (certCollection.Count == 0)
            {
                return null;
            }

            return certCollection[0];
        }

        /// <summary>
        /// Remove a publishsettings file from the store 
        /// </summary>
        /// <param name="thumbprint">The management certificate thumbprint</param>
        public static void RemoveFromStore(string thumbprint, StoreLocation location = StoreLocation.CurrentUser)
        {
            X509Certificate2 certificate = FromStore(thumbprint);
            var store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadWrite);
            store.Remove(certificate);
            store.Close();
        }

        /// <summary>
        /// Gets the certificate data from a file - can be a .cer file or publishsettings - exports the private key with the certificate
        /// </summary>
        public X509Certificate2 GetCertificateFromFile(string managementCertificate = null, StoreLocation location = StoreLocation.CurrentUser)
        {
            var storageFlagKeySet = location == StoreLocation.CurrentUser ? X509KeyStorageFlags.UserKeySet : X509KeyStorageFlags.MachineKeySet;
            // settings downloaded from https://windows.azure.com/download/publishprofile.aspx
            byte[] certBytes = SchemaVersion == 1
                ? Convert.FromBase64String(
                    _publishSettingsFileXml.Descendants("PublishProfile")
                        .Single()
                        .Attribute("ManagementCertificate")
                        .Value)
                : Convert.FromBase64String(
                    _publishSettingsFileXml.Descendants("Subscription")
                        .FirstOrDefault()
                        .Attribute("ManagementCertificate")
                        .Value);
            // assume password is empty here!
            if (!String.IsNullOrEmpty(managementCertificate))
            {
                certBytes = Convert.FromBase64String(managementCertificate);
            }
            return new X509Certificate2(certBytes, string.Empty, X509KeyStorageFlags.Exportable|X509KeyStorageFlags.PersistKeySet|storageFlagKeySet);
        }

        //TODO: Set a password at the start don't persist this and treat via a secure string
        public static X509Certificate2 GetCertificateFromXml(string xml, StoreLocation location = StoreLocation.CurrentUser)
        {
            var ps = GetFromXml(xml);
            return ps.GetCertificateFromFile(null, location);
        }

        /// <summary>
        /// Used to extract a certificate from a file
        /// </summary>
        public X509Certificate2 ExtractCertObject(FileType type)
        {
            return GetCertificateFromFile();
        }

        /// <summary>
        /// Used to extract a pfk from a file
        /// </summary>
        public X509Certificate2 ExtractPfxObject(string password)
        {
            var collection = new X509Certificate2Collection();
            collection.Import(_publishSettingsFile, password, X509KeyStorageFlags.PersistKeySet);
            return collection[0];
        }
    }
}