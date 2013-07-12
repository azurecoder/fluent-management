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
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Used to contain an SSH key whether public or key pair
    /// </summary>
    public class SSHKey : ICustomXmlSerializer
    {
        private readonly KeyType _keyType;
        /// <summary>
        /// creates an serialises an ssh key
        /// </summary>
        public SSHKey(KeyType keyType)
        {
            _keyType = keyType;
        }
        /// <summary>
        /// The key fingerprint
        /// </summary>
        public string FingerPrint { get; set; }
        /// <summary>
        /// The path to the key
        /// </summary>
        public string Path { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            if(String.IsNullOrEmpty(FingerPrint) || String.IsNullOrEmpty(Path))
                throw new FluentManagementException("The fingerprint or path has to have a value", "SSHKey");

            var element = new XElement(Namespaces.NsWindowsAzure + _keyType.ToString());
            var fingerPrint = new XElement(Namespaces.NsWindowsAzure + "FingerPrint", FingerPrint);
            var path = new XElement(Namespaces.NsWindowsAzure + "Path", Path);
            element.Add(fingerPrint);
            element.Add(path);

            return element;
        }

        #endregion
    }
}