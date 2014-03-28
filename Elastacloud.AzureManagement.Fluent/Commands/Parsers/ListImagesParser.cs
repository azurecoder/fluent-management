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
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Parses the response of a list of disks within a subscription
    /// </summary>
    internal class ListImagesParser : BaseParser
    {
        public ListImagesParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<ImageProperties>();
        }

        internal override void Parse()
        {
            /*
             * <Images xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                  <OSImage>
                    <AffinityGroup>name-of-the-affinity-group</AffinityGroup>
                    <Category>category-of-the-image</Category>
                    <Label>image-description</Label>
                    <Location>geo-location-of-the-stored-image</Location>
                    <LogicalSizeInGB>size-of-the-image</LogicalSizeInGB>
                    <MediaLink>url-of-the-containing-blob</MediaLink>
                    <Name>image-name</Name>
                    <OS>operating-system-of-the-image</OS>
                    <Eula>image-eula</Eula>
                    <Description>image-description</Description>
                    <ImageFamily>image-family</ImageFamily>
                    <ShowInGui>true|false</ShowInGui>
                    <PublishedDate>published-date</PublishedDate>
                    <IsPremium>true|false</IsPremium>
                    <PrivacyUri>uri-of-privacy-policy</PrivacyUri>
                    <RecommendedVMSize>size-of-the-virtual-machine</RecommendedVMSize>
                    <PublisherName>publisher-identifier</PublisherName>
                    <PricingDetailLink>pricing-details</PricingDetailLink>
                    <SmallIconUri>uri-of-icon</SmallIconUri>
                    <Language>language-of-image</Language>
                  </OSImage>
                  …
                </Images>*/
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + "Images")
                .Elements(GetSchema() + "OSImage");
            foreach (XElement osDetail in rootElements)
            {
                //if (osDetail.Element(GetSchema() + "MediaLink") == null)
                //{
                //    continue;
                //}

                //if (osDetail.Element(GetSchema() + "MediaLink").Value == String.Empty)
                //{
                //    continue;
                //}

                if (osDetail.Element(GetSchema() + "ImageFamily") != null)
                {
                    if (osDetail.Element(GetSchema() + "ImageFamily").Value.Contains("RightScale"))
                    {
                        continue;
                    }
                }
                var imageProperties = new ImageProperties();
                // get the affinity group if it exists
                if (osDetail.Element(GetSchema() + "Description") != null)
                {
                    imageProperties.Description = osDetail.Element(GetSchema() + "Description").Value;
                }
                if (osDetail.Element(GetSchema() + "Name") != null)
                {
                    imageProperties.Name = osDetail.Element(GetSchema() + "Name").Value;
                }
                if (osDetail.Element(GetSchema() + "Label") != null)
                {
                    imageProperties.Label = osDetail.Element(GetSchema() + "Label").Value;
                }
                if (osDetail.Element(GetSchema() + "Eula") != null)
                {
                    imageProperties.Eula = osDetail.Element(GetSchema() + "Eula").Value;
                }
                if (osDetail.Element(GetSchema() + "ImageFamily") != null)
                {
                    imageProperties.ImageFamily = osDetail.Element(GetSchema() + "ImageFamily").Value;
                }
                if (osDetail.Element(GetSchema() + "IsPremium") != null)
                {
                    imageProperties.IsPremium = bool.Parse(osDetail.Element(GetSchema() + "IsPremium").Value);
                }
                if (osDetail.Element(GetSchema() + "ShowInGui") != null)
                {
                    imageProperties.ShowInGui = bool.Parse(osDetail.Element(GetSchema() + "ShowInGui").Value);
                }
                if (osDetail.Element(GetSchema() + "MediaLink") != null)
                {
                    imageProperties.MediaLink = osDetail.Element(GetSchema() + "MediaLink").Value;
                }
                if (osDetail.Element(GetSchema() + "OS") != null)
                {
                    imageProperties.OperatingSystem = (PlatformType)Enum.Parse(typeof(PlatformType), osDetail.Element(GetSchema() + "OS").Value);
                }
                if (osDetail.Element(GetSchema() + "PublishedDate") != null)
                {
                    imageProperties.PublishedDate = DateTime.Parse(osDetail.Element(GetSchema() + "PublishedDate").Value);
                }

                CommandResponse.Add(imageProperties);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return "Images"; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}