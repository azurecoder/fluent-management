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
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary>
    /// Used to read base64 config data and encode in UTF8
    /// </summary>
    public class Base64ConfigReader
    {
        private readonly string _file;

        /// <summary>
        /// Takes a filepath parameter
        /// </summary>
        public Base64ConfigReader(string file)
        {
            _file = file;
        }

        /// <summary>
        /// Gets the contents of a file from the filpath and then re-encodes back to base64/utf and exports as string 
        /// </summary>
        /// <returns></returns>
        public string GetFileContents()
        {
            string plainText;
            if (!File.Exists(_file))
            {
                throw new ApplicationException("config file does not exist");
            }
            using (var reader = new StreamReader(_file))
            {
                plainText = reader.ReadToEnd();
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}