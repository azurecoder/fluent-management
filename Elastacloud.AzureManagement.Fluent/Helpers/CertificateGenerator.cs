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
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary>
    /// Certificate generator class used to create an X509 cert with exportable privatekey and optionally add to the personal store
    /// </summary>
    public class CertificateGenerator
    {
        public CertificateGenerator()
        {
        }

        public CertificateGenerator(string subscriptionId, X509Certificate2 managementCertificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = managementCertificate;
        }

        public X509Certificate2 ManagementCertificate { get; set; }

        public string SubscriptionId { get; set; }

        /// <summary>
        /// Static method used to create a certificate and return as a .net object
        /// </summary>
        public X509Certificate2 Create(string name, DateTime start, DateTime end, string userPassword, bool addtoStore = false)
        {
            UserPassword = userPassword ?? String.Empty;
            // generate a key pair using RSA
            var generator = new RsaKeyPairGenerator();
            // keys have to be a minimum of 2048 bits for Azure
            generator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 2048));
            KeyPair = generator.GenerateKeyPair();
            // get a copy of the private key
            AsymmetricKeyParameter privateKey = KeyPair.Private;

            // create the CN using the name passed in and create a unique serial number for the cert
            var certName = new X509Name("CN=" + name);
            BigInteger serialNo = BigInteger.ProbablePrime(120, new Random());

            // start the generator and set CN/DN and serial number and valid period
            var x509Generator = new X509V3CertificateGenerator();
            x509Generator.SetSerialNumber(serialNo);
            x509Generator.SetSubjectDN(certName);
            x509Generator.SetIssuerDN(certName);
            x509Generator.SetNotBefore(start);
            x509Generator.SetNotAfter(end);
            // add the server authentication key usage
            var keyUsage = new KeyUsage(KeyUsage.KeyEncipherment);
            x509Generator.AddExtension(X509Extensions.KeyUsage, false, keyUsage.ToAsn1Object());
            var extendedKeyUsage = new ExtendedKeyUsage(new[] {KeyPurposeID.IdKPServerAuth});
            x509Generator.AddExtension(X509Extensions.ExtendedKeyUsage, true, extendedKeyUsage.ToAsn1Object());
            // algorithm can only be SHA1 ??
            x509Generator.SetSignatureAlgorithm("sha1WithRSA");

            // Set the key pair
            x509Generator.SetPublicKey(KeyPair.Public);
            X509Certificate certificate = x509Generator.Generate(KeyPair.Private);
            
            // export the certificate bytes
            byte[] certStream = DotNetUtilities.ToX509Certificate(certificate).Export(X509ContentType.Pkcs12, UserPassword);

            // build the key parameter and the certificate entry
            var keyEntry = new AsymmetricKeyEntry(privateKey);
            var entry = new X509CertificateEntry(certificate);
            // build the PKCS#12 store to encapsulate the certificate
            var builder = new Pkcs12StoreBuilder();
            builder.SetUseDerEncoding(true);
            builder.SetCertAlgorithm(PkcsObjectIdentifiers.Sha1WithRsaEncryption);
            builder.SetKeyAlgorithm(PkcsObjectIdentifiers.Sha1WithRsaEncryption);
            builder.Build();
            // create a memorystream to hold the output 
            var stream = new MemoryStream(10000);
            // create the individual store and set two entries for cert and key
            var store = new Pkcs12Store();
            store.SetCertificateEntry("Created by Fluent Management", entry);
            store.SetKeyEntry("Created by Fluent Management", keyEntry, new[] { entry });
            store.Save(stream, UserPassword.ToCharArray(), new SecureRandom());

             // Create the equivalent C# representation
            DerEncodedCertificate = new X509Certificate2(stream.GetBuffer(), userPassword, X509KeyStorageFlags.Exportable);
           
            // if specified then this add this certificate to the store
            if (addtoStore)
                AddToMyStore(DerEncodedCertificate);

            return DerEncodedCertificate;
        }

        public X509Certificate2 DerEncodedCertificate { get; private set; }
        public string UserPassword { get; private set; }
        public AsymmetricCipherKeyPair KeyPair { get; private set; }
        /// <summary>
        /// Used to export the certificates to the filesystem 
        /// </summary>        
        public void ExportToFileSystem(string exportDirectory)
        {
            // set up the PEM writer too 
            if (exportDirectory != null)
            {
                // if this export directory doesn't exist then create it
                if (!Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }
                
                using (var writer = new StreamWriter(Path.Combine(exportDirectory, DerEncodedCertificate.Thumbprint + ".pem")))
                {
                    writer.WriteLine(GetPemData());
                }
                // also export the certs - first the .pfx
                using (var writer = new FileStream(Path.Combine(exportDirectory, DerEncodedCertificate.Thumbprint + ".pfx"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    writer.Write(GetPfxData(), 0, GetPfxData().Length);
                }
                // also export the certs - then the .cer
                using (var writer = new FileStream(Path.Combine(exportDirectory, DerEncodedCertificate.Thumbprint + ".cer"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    writer.Write(GetCerData(), 0, GetCerData().Length);
                }
            }
        }

        public string GetPemData()
        {
            var textWriter = new StringWriter();
            var pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(KeyPair.Private, "DESEDE", UserPassword.ToCharArray(), new SecureRandom());
            pemWriter.Writer.Flush();
            return textWriter.ToString();
        }

        public byte[] GetPfxData()
        {
            return DerEncodedCertificate.Export(X509ContentType.Pfx, UserPassword);
        }

        public byte[] GetCerData()
        {
            return DerEncodedCertificate.Export(X509ContentType.Cert);
        }

        public void ExportToStorageAccount(string account, string container, string folder)
        {
            if(String.IsNullOrEmpty(SubscriptionId) && ManagementCertificate == null)
                throw new FluentManagementException("please provide a subscription id and management certificate to continue", "CertificateGenerator");

            var client = new StorageClient(SubscriptionId, ManagementCertificate);
            var storageKeys = client.GetStorageAccountKeys(account);
            var key = storageKeys[0];

            var credentials = new CloudStorageAccount(new StorageCredentials(account, key), true);
            var blobClient = credentials.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(container);
            blobContainer.CreateIfNotExists();

            var pemBlob = blobContainer.GetBlockBlobReference(String.Format("{0}/{1}.{2}.pem", folder, DerEncodedCertificate.Thumbprint, DateTime.UtcNow.ToString("ddMMyyyy")));
            pemBlob.UploadText(GetPemData());

            var pfxBlob = blobContainer.GetBlockBlobReference(String.Format("{0}/{1}.{2}.pfx", folder, DerEncodedCertificate.Thumbprint, DateTime.UtcNow.ToString("ddMMyyyy")));
            pfxBlob.UploadFromByteArray(GetPfxData(), 0, GetPfxData().Count());

            var cerBlob = blobContainer.GetBlockBlobReference(String.Format("{0}/{1}.{2}.cer", folder, DerEncodedCertificate.Thumbprint, DateTime.UtcNow.ToString("ddMMyyyy")));
            cerBlob.UploadFromByteArray(GetCerData(), 0, GetCerData().Count());
        }

        #region Certificate Store
        
        /// <summary>
        /// Adds a certificate to My store for the LocalMachine
        /// </summary>
        public void AddToMyStore(X509Certificate2 certificate2)
        {
            X509Store store = ReturnStore();
            store.Add(certificate2);
            store.Close();
        }

        /// <summary>
        /// Returns the My LocalMachine store
        /// </summary>
        /// <returns></returns>
        private X509Store ReturnStore()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
            return store;
        }

        /// <summary>
        /// Removes a certificate from My LocalMachine Store
        /// </summary>
        public void RemoveFromMyStore(X509Certificate2 certificate2)
        {
            X509Store store = ReturnStore();
            store.Remove(certificate2);
            store.Close();
        }

        /// <summary>
        /// Checks to see whether the certificate is in the appropriate store
        /// </summary>
        public bool IsInMyStore(X509Certificate2 certificate2)
        {
            X509Store store = ReturnStore();
            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,
                                                                                certificate2.Thumbprint, false);
            return certCollection.Count > 0;
        }

        #endregion
    }
}