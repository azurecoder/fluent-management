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
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;


namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// The configuration set which contains information about the Linux VM
    /// </summary>
    public class LinuxConfigurationSet : ConfigurationSet
    {
        /// <summary>
        /// Constructs a linux configuration set
        /// </summary>
        public LinuxConfigurationSet()
        {
        }
        /// <summary>
        /// The hostname of the Linux VM
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// The username which will access the Linux VM
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The UserPassword which will be sued access the Linux VM
        /// </summary>
        public string UserPassword { get; set; }
        /// <summary>
        /// Whether or not to disable password authentication in favour of SSH
        /// </summary>
        public bool DisableSshPasswordAuthentication { get; set; }
        /// <summary>
        /// A list of public keys which are sent to the server 
        /// </summary>
        public List<SSHKey> PublicKeys { get; set; }
        /// <summary>
        /// A list of key pairs which are sent to the server this and the above property are derived from the 
        /// </summary>
        public List<SSHKey> KeyPairs { get; set; }

        #region Overrides of ConfigurationSet

        /// <summary>
        /// Used to set the type of configuration set used with this vm instance
        /// </summary>
        public override ConfigurationSetType ConfigurationSetType
        {
            get { return ConfigurationSetType.LinuxProvisioningConfiguration; }
        }

        #endregion

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public override XElement GetXmlTree()
        {
            /*
        <ConfigurationSet> 
          <ConfigurationSetType>LinuxProvisioningConfiguration</ConfigurationSetType>
          <HostName>host-name-for-the-vm</HostName>
          <UserName>new-user-name</UserName> 
          <UserPassword>password-for-the-new-user</UserPassword> 
          <DisableSshPasswordAuthentication>true|false</DisableSshPasswordAuthentication>           
          <SSH>
            <PublicKeys>
              <PublicKey>
                <FingerPrint>certificate-fingerprint</FingerPrint>
                <Path>SSH-public-key-storage-location</Path>     
              </PublicKey>
            </PublicKeys>
            <KeyPairs>
              <KeyPair>
                <FingerPrint>certificate-fingerprint</FinguerPrint>
                <Path>SSH-public-key-storage-location</Path>
              </KeyPair>
            </KeyPairs>
          </SSH>
        </ConfigurationSet>*/

            var element = new XElement(Namespaces.NsWindowsAzure + "ConfigurationSet",
                                       new XElement(Namespaces.NsWindowsAzure + "ConfigurationSetType",
                                                    ConfigurationSetType.ToString()));
            if (!String.IsNullOrEmpty(HostName))
            {
                element.Add(new XElement(Namespaces.NsWindowsAzure + "HostName", HostName));
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                element.Add(new XElement(Namespaces.NsWindowsAzure + "UserName", UserName));
            }
            if (!String.IsNullOrEmpty(UserPassword))
            {
                element.Add(new XElement(Namespaces.NsWindowsAzure + "UserPassword", UserPassword));
            }
            element.Add(new XElement(Namespaces.NsWindowsAzure + "DisableSshPasswordAuthentication",
                                     DisableSshPasswordAuthentication.ToString().ToLower()));

            XElement sshElement = null;
            if (PublicKeys != null && PublicKeys.Count > 0)
            {
                sshElement = new XElement(Namespaces.NsWindowsAzure + "SSH" /* add the public key and keypairs xml */);
                // add the public keys section
                if (PublicKeys != null && PublicKeys.Count > 0)
                {
                    var publicKeys = new XElement(Namespaces.NsWindowsAzure + "PublicKeys");
                    foreach (var publicKey in PublicKeys)
                    {
                        publicKeys.Add(publicKey.GetXmlTree());
                    }
                    sshElement.Add(publicKeys);
                }
            }

            // check to see whether the keys can be appended
            if (KeyPairs != null && KeyPairs.Count > 0)
            {
                if (sshElement == null)
                {
                    sshElement = new XElement(Namespaces.NsWindowsAzure + "SSH"
                        /* add the public key and keypairs xml */);
                }
                // add the keypairs section to the node tree

                var keyPairs = new XElement(Namespaces.NsWindowsAzure + "KeyPairs");
                foreach (var keyPair in KeyPairs)
                {
                    keyPairs.Add(keyPair.GetXmlTree());
                }
                sshElement.Add(keyPairs);
            }

            if (sshElement != null)
            {
                element.Add(sshElement);
            }

            return element;
        }

        #endregion
    }
}
