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
//using Chilkat;
using Chilkat;

namespace Elastacloud.AzureManagement.LinuxKeyConverter
{
    /// <summary>
    /// Used to convert SSH keys between formats
    /// </summary>
// ReSharper disable once InconsistentNaming
    public class SSHKeyConverter : IKeyConverter
    {
        private readonly string _privateKeyFile;
        private readonly string _password;
        private bool _converted = false;

        /// <summary>
        /// Constructs an SSH conversion path for keys
        /// </summary>
        public SSHKeyConverter(string privateKeyFile, string password)
        {
            _privateKeyFile = privateKeyFile;
            _password = password;
        }
        /// <summary>
        /// Converts an SSH key between .pem and opensshv2 formats
        /// </summary>
        /// <returns>conversion succeeded</returns>
        public bool Convert()
        {
            // get the file contents
            string fileContent = string.Empty;
            using (var reader = new StreamReader(_privateKeyFile))
            {
                fileContent = reader.ReadToEnd();
            }
            // check to see whether this has worked
            var key = new SshKey { Password = _password };
            // get the ssh key 
            bool converted = key.FromOpenSshPrivateKey(fileContent);
            // if this hasn't worked then just drop out of this method
            if (!converted)
                return false;
            // update the ssh key
            string content = key.ToOpenSshPrivateKey(false);
            // get the filename without the key extension
            string openSsh2Filename = Path.GetFileNameWithoutExtension(_privateKeyFile) + ".pvk";
            // get the path 
            string openSsh2Directory = Path.GetDirectoryName(_privateKeyFile);

            // get the full path 
            KeyFilePath = Path.Combine(openSsh2Directory, openSsh2Filename);
            // create the file
            using (var writer = new StreamWriter(File.Create(KeyFilePath)))
            {
                writer.Write(content);
            }
            // if that worked return out of this 
            return _converted = true;
        }

        /// <summary>
        /// Gets the new key filepath
        /// </summary>
        public string KeyFilePath { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return ConvertKey();
        }

        private string ConvertKey()
        {
            if (_converted)
                Convert();

            if (!_converted)
                throw new ApplicationException("key convert failed no available file output for openssh2 keys");

            string fileContents;
            using (var reader = new StreamReader(KeyFilePath))
            {
                fileContents = reader.ReadToEnd();
            }

            return fileContents;
        }
    }
}
