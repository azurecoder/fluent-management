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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Used to parse the Xml response back from the <Sites></Sites>
    /// </summary>
    internal class WebsiteParser : BaseParser
    {
        /// <summary>
        /// Used to construct a website parser
        /// </summary>
        /// <param name="document"></param>
        public WebsiteParser(XDocument document) : base(document)
        {
            CommandResponse = new List<Website>();
        }

        /// <summary>
        /// Contains a collections of websites
        /// </summary>
        public List<Website> Websites { get; private set; }
        /// <summary>
        /// The namespace that contains details in an array string value
        /// </summary>
        public const string SerialisationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return WebsiteParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        internal override void Parse()
        {
            if (Websites == null)
                Websites = new List<Website>();

            XNamespace serialisationNamespace = SerialisationNamespace;
            XElement rootElements = Document.Element(GetSchema() + RootElement);
            foreach (var element in rootElements.Elements(GetSchema() + "Site"))
            {
                var site = new Website() {Hostname = new List<string>()};
                // get the compute modefrom the instance 
                if (element.Element(GetSchema() + "ComputeMode") != null)
                {
                    site.ComputeMode = (ComputeMode)Enum.Parse(typeof(ComputeMode), element.Element(GetSchema() + "ComputeMode").Value, true);
                }
                // get the name of the site as in xxxx.azurewebsites.net
                if (element.Element(GetSchema() + "Name") != null)
                {
                    site.Name = element.Element(GetSchema() + "Name").Value;
                }
                // test to see whether the sitehas been enabled
                if (element.Element(GetSchema() + "Enabled") != null)
                {
                    site.Enabled = bool.Parse(element.Element(GetSchema() + "Enabled").Value);
                }
                // get the underlying webspace that this site is in
                if (element.Element(GetSchema() + "WebSpace") != null)
                {
                    site.Webspace = element.Element(GetSchema() + "WebSpace").Value;
                }
                // get the usage of this website
                if (element.Element(GetSchema() + "UsageState") != null)
                {
                    site.Usage = (WebsiteUsage)Enum.Parse(typeof(WebsiteUsage), element.Element(GetSchema() + "UsageState").Value, true);
                }
                // get the state of the website
                if (element.Element(GetSchema() + "State") != null)
                {
                    site.State = (WebsiteState)Enum.Parse(typeof(WebsiteState), element.Element(GetSchema() + "State").Value, true);
                }
                // get the hostnames using the serialisation namespace
                // EnabledHostNames [ xmlns:a= ]
                if (element.Element(GetSchema() + "EnabledHostNames") != null)
                {
                    foreach (var hostNameElement in element.Element(GetSchema() + "EnabledHostNames").
                        Elements(serialisationNamespace + "string"))
                    {
                        site.Hostname.Add(hostNameElement.Value);
                    }
                }
                // get the state of the website
                if (element.Element(GetSchema() + "SiteMode") != null)
                {
                    site.Mode = (SiteMode)Enum.Parse(typeof(SiteMode), element.Element(GetSchema() + "SiteMode").Value, true);
                }
               
                // get the state of the website
                if (element.Element(GetSchema() + "ComputeMode") != null)
                {
                    site.ComputeMode = (ComputeMode)Enum.Parse(typeof(ComputeMode), element.Element(GetSchema() + "ComputeMode").Value);
                }
                Websites.Add(site);
            }
           
            CommandResponse = Websites;
        }

        #endregion
    }
}