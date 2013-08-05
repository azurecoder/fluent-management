/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.IO;

namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    /// <summary>
    /// Used to convert SSH keys between formats
    /// </summary>
    public class SSHKeyConverter : IKeyConverter
    {
        private readonly string _privateKeyFile;
        private readonly string _password;

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
           
            return false;
        }

        /// <summary>
        /// Gets the new key filepath
        /// </summary>
        public string KeyFilePath { get; private set; }
    }

    /// <summary>
    /// Converts an SSH key between formats
    /// </summary>
    public interface IKeyConverter
    {
        /// <summary>
        /// Converts an SSH key between .pem and opensshv2 formats
        /// </summary>
        /// <returns>conversion succeeded</returns>
        bool Convert();
        /// <summary>
        /// Gets the new key filepath
        /// </summary>
        string KeyFilePath { get; }
    }
}
