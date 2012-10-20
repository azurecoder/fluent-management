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
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Properties;

namespace Elastacloud.AzureManagement.Fluent.Helpers
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
        public PublishSettingsExtractor(string filePath)
        {
            _publishSettingsFile = filePath;
            _publishSettingsFileXml = XDocument.Load(filePath);
        }
        /// <summary>
        /// Gets a list of subscriptions in a .publishsettings file
        /// </summary>
        /// <param name="file">The path to the file</param>
        /// <returns>a list of subscriptions</returns>
        public Dictionary<string, string> GetSubscriptions()
        {
            return _publishSettingsFileXml.Descendants("Subscription").ToDictionary(a => a.Attribute("Id").Value, a => a.Attribute("Name").Value);
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
        public X509Certificate2 GetCertificateFromFile(StoreLocation location = StoreLocation.CurrentUser)
        {
            var storageFlagKeySet = location == StoreLocation.CurrentUser ? X509KeyStorageFlags.UserKeySet : X509KeyStorageFlags.MachineKeySet;
            // settings downloaded from https://windows.azure.com/download/publishprofile.aspx
            byte[] certBytes =
                Convert.FromBase64String(_publishSettingsFileXml.Descendants("PublishProfile").Single().Attribute("ManagementCertificate").Value);
            return new X509Certificate2(certBytes, string.Empty, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | storageFlagKeySet);
        }

        //TODO: Set a password at the start don't persist this and treat via a secure string
        public static X509Certificate2 GetCertificateFromXml(string xml, StoreLocation location = StoreLocation.CurrentUser)
        {
            var storageFlagKeySet = location == StoreLocation.CurrentUser ? X509KeyStorageFlags.UserKeySet : X509KeyStorageFlags.MachineKeySet;
            XDocument doc = XDocument.Parse(xml);
            byte[] certBytes =
                Convert.FromBase64String(
                    doc.Descendants("PublishProfile").Single().Attribute("ManagementCertificate").Value);
            //return new X509Certificate2(certBytes, String.Empty, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
            return new X509Certificate2(certBytes, string.Empty, X509KeyStorageFlags.Exportable|X509KeyStorageFlags.PersistKeySet|storageFlagKeySet);
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