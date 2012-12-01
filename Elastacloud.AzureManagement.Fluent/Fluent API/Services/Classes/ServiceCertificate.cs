/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    /// <summary>
    /// Encapsulates a Windows Azure service certificate
    /// </summary>
    public class ServiceCertificate
    {
        /// <summary>
        /// Constructor to set default properties validity 1 year from today and default pvk "password"
        /// </summary>
        public ServiceCertificate(string name)
        {
            Name = name;
            ValidFrom = DateTime.Now;
            ValidTo = DateTime.Now.AddYears(1);
            UseExisting = false;
            PvkPassword = "password";
        }

        /// <summary>
        /// Constructor used for an existing service certificate
        /// </summary>
        public ServiceCertificate()
        {
            UseExisting = true;
        }

        /// <summary>
        /// The DN of the cert used for both the subject and issuer - normally *.cloudapp.net
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// When it is valid from
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// When it expires
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// The private key password
        /// </summary>
        public string PvkPassword { get; set; }

        /// <summary>
        /// The Service Certificate
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Provides the config enablement for SSL
        /// </summary>
        public bool SslEnabled { get; set; }

        /// <summary>
        /// This will use an existing certificate given a password and thumbprint
        /// </summary>
        public bool UseExisting { get; set; }

        /// <summary>
        /// Provides the config enablement for remote desktop
        /// </summary>
        public bool RemoteDesktopEnabled { get; set; }

        /// <summary>
        /// Used to create the Cert
        /// </summary>
        public X509Certificate2 Create()
        {
            return (Certificate = CertificateGenerator.Create(Name, ValidFrom, ValidTo, PvkPassword, true));
        }

        /// <summary>
        /// Gets an existing certificate given a thumbprint from the Local Stores
        /// </summary>
        public X509Certificate2 GetExisting(string thumbprint, string password)
        {
            PvkPassword = password;
            return (Certificate = PublishSettingsExtractor.FromStore(thumbprint));
        }
    }
}