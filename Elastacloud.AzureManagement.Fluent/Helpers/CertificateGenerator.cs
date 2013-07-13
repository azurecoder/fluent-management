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
using System.Security.Cryptography.X509Certificates;
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
        /// <summary>
        /// Static method used to create a certificate and return as a .net object
        /// </summary>
        public static X509Certificate2 Create(string name, DateTime start, DateTime end, string userPassword, bool addtoStore = false, string exportDirectory = null)
        {
            // generate a key pair using RSA
            var generator = new RsaKeyPairGenerator();
            // keys have to be a minimum of 2048 bits for Azure
            generator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 2048));
            AsymmetricCipherKeyPair cerKp = generator.GenerateKeyPair();
            // get a copy of the private key
            AsymmetricKeyParameter privateKey = cerKp.Private;

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
            x509Generator.SetPublicKey(cerKp.Public);
            X509Certificate certificate = x509Generator.Generate(cerKp.Private);
            

            // export the certificate bytes
            byte[] certStream = DotNetUtilities.ToX509Certificate(certificate).Export(X509ContentType.Pkcs12, userPassword);

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
            store.Save(stream, userPassword.ToCharArray(), new SecureRandom());

             // Create the equivalent C# representation
            var cert = new X509Certificate2(stream.GetBuffer(), userPassword, X509KeyStorageFlags.Exportable);
            // set up the PEM writer too 
            if (exportDirectory != null)
            {
                var textWriter = new StringWriter();
                var pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(cerKp.Private, "DESEDE", userPassword.ToCharArray(), new SecureRandom());
                pemWriter.Writer.Flush();
                string privateKeyPem = textWriter.ToString();
                using (var writer = new StreamWriter(Path.Combine(exportDirectory, cert.Thumbprint + ".pem")))
                    {
                        writer.WriteLine(privateKeyPem);
                    }
                // also export the certs - first the .pfx
                byte[] privateKeyBytes = cert.Export(X509ContentType.Pfx, userPassword);
                using (var writer = new FileStream(Path.Combine(exportDirectory, cert.Thumbprint + ".pfx"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    writer.Write(privateKeyBytes, 0, privateKeyBytes.Length);
                }
                // also export the certs - then the .cer
                byte[] publicKeyBytes = cert.Export(X509ContentType.Cert);
                using (var writer = new FileStream(Path.Combine(exportDirectory, cert.Thumbprint + ".cer"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    writer.Write(publicKeyBytes, 0, publicKeyBytes.Length);
                }
            }

            // if specified then this add this certificate to the store
            if (addtoStore)
                AddToMyStore(cert);

            return cert;
        }

        #region Certificate Store
        
        /// <summary>
        /// Adds a certificate to My store for the LocalMachine
        /// </summary>
        public static void AddToMyStore(X509Certificate2 certificate2)
        {
            X509Store store = ReturnStore();
            store.Add(certificate2);
            store.Close();
        }

        /// <summary>
        /// Returns the My LocalMachine store
        /// </summary>
        /// <returns></returns>
        private static X509Store ReturnStore()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
            return store;
        }

        /// <summary>
        /// Removes a certificate from My LocalMachine Store
        /// </summary>
        public static void RemoveFromMyStore(X509Certificate2 certificate2)
        {
            X509Store store = ReturnStore();
            store.Remove(certificate2);
            store.Close();
        }

        /// <summary>
        /// Checks to see whether the certificate is in the appropriate store
        /// </summary>
        public static bool IsInMyStore(X509Certificate2 certificate2)
        {
            X509Store store = ReturnStore();
            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,
                                                                                certificate2.Thumbprint, false);
            return certCollection.Count > 0;
        }

        #endregion
    }
}