/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    ///  Used to encapsulate a .cscfg file and be able to pull properties from it 
    /// </summary>
    public class CscfgFile : ConfigurationFile
    {
        /// <summary>
        /// The single instance of the file 
        /// </summary>
        public static ConfigurationFile Instance;

        /// <summary>
        /// Default constructor used to load and find the file 
        /// </summary>
        private CscfgFile(string name) : base(name)
        {
        }

        /// <summary>
        /// Used to generate ad-hoc instances of the .cscfg file
        /// </summary>
        private CscfgFile(XDocument doc) : base(doc)
        {
        }

        /// <summary>
        /// Singleton for the file
        /// </summary>
        public static ConfigurationFile GetInstance(string name)
        {
            if (Instance == null)
                return (Instance = new CscfgFile(name));
            return Instance;
        }

        /// <summary>
        /// Used to get an ad-hoc reference to the .cscfg file to parse out the settings
        /// </summary>
        public static CscfgFile GetAdHocInstance(XDocument doc)
        {
            return new CscfgFile(doc);
        }

        /// <summary>
        /// Used to get the file instance 
        /// </summary>
        protected override string GetFileExtension()
        {
            return Constants.CscfgExtension;
        }

        /// <summary>
        /// Gets a list of role names from the .cscfg file
        /// </summary>
        public List<string> GetRoleNameList()
        {
            return OriginalVersion.Descendants(Namespaces.NsServiceManagement + "Role")
                .Attributes("name")
                .Select(attribute => attribute.Value)
                .ToList();
        }

        /// <summary>
        /// Gets a setting value for a role given a rolename and settings name
        /// </summary>
        public string GetSettingForRole(string settingName, string roleName)
        {
            return OriginalVersion.Descendants(name: Namespaces.NsServiceManagement + "Role").FirstOrDefault(a => ((string) a.Attribute("name")).ToLower() == roleName.ToLower())
                    .Descendants(Namespaces.NsServiceManagement + "Setting").FirstOrDefault(b => (string) b.Attribute("name") == settingName)
                    .Attribute("value").Value;
        }

        /// <summary>
        /// Checks to see whether a particular setting exists in the roleset
        /// </summary>
        /// <param name="settingName">The name of the setting being checked</param>
        /// <param name="roleIndex">Not implemented but can be used to go to a particular role index based on the order of the XML Cscfg file</param>
        /// <returns>A bool as to whether the setting is present or not</returns>
        public bool DoesSettingExist(string settingName, int roleIndex = 1)
        {
            return GetSetting(settingName) != null;
        }

        /// <summary>
        /// Returns a setting without needing to have a role to look up
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <returns>A string value</returns>
        public string GetSettingWithoutRole(string settingName)
        {
            var element = GetSetting(settingName);
            if (element == null)
                return null;
            return element.Attribute("value").Value;
        }
        /// <summary>
        /// Checks to see whether a particular setting exists in the roleset
        /// </summary>
        /// <param name="settingName">The name of the setting being checked</param>
        /// <returns>A bool as to whether the setting is present or not</returns>
        private XElement GetSetting(string settingName)
        {
            var setting = OriginalVersion.Descendants(name: Namespaces.NsServiceManagement + "Role").FirstOrDefault()
                .Descendants(Namespaces.NsServiceManagement + "Setting").FirstOrDefault(
                    b => (string)b.Attribute("name") == settingName);
            return setting;
        }

        /// <summary>
        /// Updates an existing setting for a role with the predetermined value 
        /// </summary>
        public void UpdateSettingForRole(string settingName, string settingValue, string roleName)
        {
            NewVersion.Descendants(name: Namespaces.NsServiceManagement + "Role").FirstOrDefault(
                a => (string) a.Attribute("name") == roleName)
                .Descendants(Namespaces.NsServiceManagement + "Setting").FirstOrDefault(
                    b => (string) b.Attribute("name") == settingName)
                .SetAttributeValue("value", settingValue);
        }

        /// <summary>
        /// Gets the instance count for a particular role name
        /// </summary>
        public int GetInstanceCountForRole(string roleName)
        {
            var elementForRole = GetInstanceCountElementForRole(roleName);
            return int.Parse(elementForRole.Attribute("count").Value);
        }

        /// <summary>
        /// Sets an instance count attribute value for a particular role
        /// </summary>
        /// <param name="roleName">the name of the role</param>
        /// <param name="instanceCount">the number of instance of the role</param>
        public void SetInstanceCountForRole(string roleName, int instanceCount)
        {
            var elementForRole = GetInstanceCountElementForRole(roleName);
            elementForRole.SetAttributeValue("count", instanceCount);
        }

        /// <summary>
        /// Gets the instance count for the role
        /// </summary>
        /// <param name="roleName">The name of the role</param>
        /// <returns>An XElement instance of the instance count element</returns>
        private XElement GetInstanceCountElementForRole(string roleName)
        {
            XElement role = OriginalVersion.Descendants(Namespaces.NsServiceManagement + "Role")
               .FirstOrDefault(a => (string)a.Attribute("name") == roleName);
            // updates the instance count number here
            return role.Elements(Namespaces.NsServiceManagement + "Instances")
                .FirstOrDefault();
        }

    }
}