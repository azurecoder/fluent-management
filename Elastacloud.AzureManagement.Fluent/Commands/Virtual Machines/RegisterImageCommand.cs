/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Globalization;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    ///   Registers a virtual machine image for either Linux or Windowss     
    ///  </summary>
    public class RegisterImageCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/images        
        /// <summary>
        ///   Registers a virtual machine image for either Linux or Windowss     
        ///  </summary>
        internal RegisterImageCommand(ImageProperties properties)
        {
            AdditionalHeaders["x-ms-version"] = "2012-08-01";
            OperationId = "images";
            ServiceType = "services";
            HttpVerb = HttpVerbPost;
            properties.PublishedDate = DateTime.UtcNow;
            properties.IsPremium = properties.IsPremium.HasValue && properties.IsPremium.Value;
            properties.ShowInGui = properties.ShowInGui.HasValue && properties.ShowInGui.Value; 
            Properties = properties;
        }

        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public ImageProperties Properties { get; set; }

        /// <summary>
        /// Creates a deployment payload for a predefined template 
        /// </summary>
        /// <returns>A string xml representation</returns>
        /// POST https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/<deployment-name>/roleinstances/<role-name>/Operations
        protected override string CreatePayload()
        {
            /*<OSImage xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
               <Label>image-label</Label>
               <MediaLink>uri-of-the-containing-blob</MediaLink>
               <Name>image-name</Name>
               <OS>Linux|Windows</OS>
               <Eula>image-eula</Eula>
               <Description>image-description</Description>
               <ImageFamily>image-family</ImageFamily>
               <PublishedDate>published-date</PublishedDate>
               <IsPremium>true/false</IsPremium>
               <ShowInGui>true/false</ShowInGui>
               <PrivacyUri>http://www.example.com/privacypolicy.html</PrivacyUri>
               <IconUri>http://www.example.com/favicon.png</IconUri>
               <RecommendedVMSize>Small/Large/Medium/ExtraLarge</RecommendedVMSize>
               <SmallIconUri>http://www.example.com/smallfavicon.png</SmallIconUri>
               <Language>language-of-image</Language>
            </OSImage>*/
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "OSImage",
                             new XElement(ns + "Label", Properties.Label),
                             new XElement(ns + "MediaLink", Properties.MediaLink),
                             new XElement(ns + "Name", Properties.Name),
                             new XElement(ns + "OS", Properties.OperatingSystem.ToString()),
                             new XElement(ns + "PublishedDate", Properties.PublishedDate.ToString(CultureInfo.InvariantCulture)),
                             new XElement(ns + "IsPremium", Properties.IsPremium.ToString().ToLowerInvariant()),
                             new XElement(ns + "ShowInGui", Properties.ShowInGui.ToString().ToLowerInvariant())));
            return doc.ToStringFullXmlDeclaration();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "RegisterImageCommand";
        }
    }
}
