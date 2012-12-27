/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class CreateMobileServiceCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal CreateMobileServiceCommand(string name, string config, string description = "Created by Fluent Management")
        {
            ServiceType = "applications";
            Name = name;
            Description = description;
            Config = config;
            HttpVerb = HttpVerbPost;
        }

        

        // POST https://management.core.windows.net/<subscription-id>/services/mobileservices/
        /// <summary>
        /// The Xml payload that is created and sent to the Fabric with the create deployment parameters
        ///  <?xml version="1.0" encoding="utf-8"?>
        ///   <Application xmlns="http://schemas.microsoft.com/windowsazure">
        ///    <Name>##name##</Name> 
        ///    <Label>##label##</Label>
        ///    <Description>##description##</Description>
        ///    <Configuration>##spec##</Configuration>
        ///   </Application>
        /// </summary>
        /// <returns>A string Xml document representation</returns>
        protected override string CreatePayload()
        {
            XNamespace ns = Namespaces.NsWindowsAzure;
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "Application",
                             new XElement(ns + "Name", Name.ToLower() + "mobileservice"),
                             new XElement(ns + "Label", Name.ToLower()),
                             new XElement(ns + "Description", Description),
                             new XElement(ns + "Configuration", Config)));
            return doc.ToStringFullXmlDeclaration();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "CreateMobileServiceCommand";
        }
    }
}