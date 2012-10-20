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
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// Abstract base class used for all types of configuration file 
    /// </summary>
    public abstract class ConfigurationFile
    {
        /// <summary>
        /// Creates a new instance of the class and checks to see whether the file exists and can be loaded
        /// </summary>
        protected ConfigurationFile(string name)
        {
            Filename = name;
            CheckFileExistsAndLoadOriginalVersion();
        }

        /// <summary>
        /// Sets the default XMl document of the configuration file
        /// </summary>
        protected ConfigurationFile(XDocument doc)
        {
            OriginalVersion = NewVersion = doc;
        }

        /// <summary>
        /// The XML document - original version before modification 
        /// </summary>
        public XDocument OriginalVersion { get; set; }

        /// <summary>
        /// The new version - after modification
        /// </summary>
        public XDocument NewVersion { get; set; }

        /// <summary>
        /// The filename to do the lookup 
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Abstract method returns the file extension
        /// </summary>
        protected abstract string GetFileExtension();

        /// <summary>
        /// Checks to see whether the original file loads correctly and 
        /// </summary>
        private void CheckFileExistsAndLoadOriginalVersion()
        {
            if (!File.Exists(Filename))
                throw new ApplicationException("configuration file does not exist");
            if (Path.GetExtension(Filename) != GetFileExtension())
                throw new ApplicationException("not a valid " + GetFileExtension() + " file");

            NewVersion = OriginalVersion = XDocument.Load(Filename);
        }

        /// <summary>
        /// Deletes and replaces the configuration - i.e. restores it back to the point it was at beforehand
        /// </summary>
        public void RollbackConfigurationFile()
        {
            string oldFilename = Path.Combine(Path.GetDirectoryName(Filename),
                                              Path.GetFileNameWithoutExtension(Filename) + ".old");
            string bakFilename = Path.Combine(Path.GetDirectoryName(Filename),
                                              Path.GetFileNameWithoutExtension(Filename) + ".000");

            File.Replace(oldFilename, Filename, bakFilename);
            File.Delete(bakFilename);
        }

        /// <summary>
        /// persists the configuration file to a well known location
        /// </summary>
        public void PersistConfigurationFile(ConfigurationFileType type)
        {
            string filePath = null;
            XDocument versionToPersist = null;
            switch (type)
            {
                case ConfigurationFileType.Current:
                    filePath = Filename;
                    versionToPersist = NewVersion;
                    break;
                case ConfigurationFileType.Backup:
                    filePath = Path.Combine(Path.GetDirectoryName(Filename),
                                            Path.GetFileNameWithoutExtension(Filename) + ".old");
                    versionToPersist = OriginalVersion;
                    break;
                case ConfigurationFileType.Replacement:
                    filePath = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileName(Filename) + ".000");
                    versionToPersist = OriginalVersion;
                    break;
            }
            // first we need to save the altered version of the .csdef file
            FileStream stream = File.Create(filePath);
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(versionToPersist.ToStringFullXmlDeclaration());
            }
        }
    }
}