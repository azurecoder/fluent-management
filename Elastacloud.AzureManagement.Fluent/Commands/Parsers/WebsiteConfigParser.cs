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
    internal class WebsiteConfigParser : BaseParser
    {
        /// <summary>
        /// Used to construct a website parser
        /// </summary>
        /// <param name="document"></param>
        public WebsiteConfigParser(XDocument document)
            : base(document)
        {
            CommandResponse = new WebsiteConfig();
        }

        /// <summary>
        /// The namespace that contains details in an array string value
        /// </summary>
        public const string SerialisationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

        #region Overrides of BaseParser

        public WebsiteConfig Config { get; set; }

        internal override string RootElement
        {
            get { return WebsiteConfigParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        internal override void Parse()
        {
            if (Config == null)
                Config = new WebsiteConfig();

            XNamespace serialisationNamespace = SerialisationNamespace;
            var element = Document.Element(GetSchema() + "SiteConfig");
            
            #region boolean values

            // get the compute modefrom the instance 
            if (element.Element(GetSchema() + "DetailedErrorLoggingEnabled") != null)
            {
                Config.DetailedErrorLoggingEnabled = bool.Parse(element.Element(GetSchema() + "DetailedErrorLoggingEnabled").Value);
            }
            // get the compute modefrom the instance 
            if (element.Element(GetSchema() + "HttpLoggingEnabled") != null)
            {
                Config.HttpLoggingEnabled = bool.Parse(element.Element(GetSchema() + "HttpLoggingEnabled").Value);
            }
            // get the compute modefrom the instance 
            if (element.Element(GetSchema() + "RequestTracingEnabled") != null)
            {
                Config.RequestTracingEnabled = bool.Parse(element.Element(GetSchema() + "RequestTracingEnabled").Value);
            }
            // get the compute modefrom the instance 
            if (element.Element(GetSchema() + "Use32BitWorkerProcess") != null)
            {
                Config.Use32BitWorkerProcess = bool.Parse(element.Element(GetSchema() + "Use32BitWorkerProcess").Value);
            }

            #endregion

            #region string and int values
                
            // get the name of the site as in xxxx.azurewebsites.net
            if (element.Element(GetSchema() + "NetFrameworkVersion") != null)
            {
                Config.NetFrameworkVersion = element.Element(GetSchema() + "NetFrameworkVersion").Value;
            }
            // get the name of the site as in xxxx.azurewebsites.net
            if (element.Element(GetSchema() + "PhpVersion") != null)
            {
                Config.PhpVersion = element.Element(GetSchema() + "PhpVersion").Value;
            }
            // get the name of the site as in xxxx.azurewebsites.net
            if (element.Element(GetSchema() + "PublishingUsername") != null)
            {
                Config.PublishingUsername = element.Element(GetSchema() + "PublishingUsername").Value;
            }
            // get the name of the site as in xxxx.azurewebsites.net
            if (element.Element(GetSchema() + "PublishingPassword") != null)
            {
                Config.PublishingPassword = element.Element(GetSchema() + "PublishingPassword").Value;
            }
            // get the name of the site as in xxxx.azurewebsites.net
            if (element.Element(GetSchema() + "NumberOfWorkers") != null)
            {
                Config.NumberOfWorkers = int.Parse(element.Element(GetSchema() + "NumberOfWorkers").Value);
            }

            #endregion

            #region collections

            // get the appsettings
            if (element.Element(GetSchema() + "AppSettings") != null)
            {
                Config.AppSettings = new Dictionary<string, string>();
                foreach (var nvPair in element.Element(GetSchema() + "AppSettings").
                    Elements(GetSchema() + "NameValuePair"))
                {
                    Config.AppSettings.Add(nvPair.Element(GetSchema() + "Name").Value, nvPair.Element(GetSchema() + "Value").Value);                        
                }
            }
            // get the metadata
            if (element.Element(GetSchema() + "Metadata") != null)
            {
                Config.Metadata = new Dictionary<string, string>();
                foreach (var nvPair in element.Element(GetSchema() + "Metadata").
                    Elements(GetSchema() + "NameValuePair"))
                {
                    Config.Metadata.Add(nvPair.Element(GetSchema() + "Name").Value, nvPair.Element(GetSchema() + "Value").Value);
                }
            }
            // get the connectionstrings
            if (element.Element(GetSchema() + "ConnectionStrings") != null)
            {
                Config.ConnectionStrings = new Dictionary<string, string>();
                foreach (var nvPair in element.Element(GetSchema() + "ConnectionStrings").
                    Elements(GetSchema() + "ConnStringInfo"))
                {
                    Config.ConnectionStrings.Add(nvPair.Element(GetSchema() + "Name").Value, nvPair.Element(GetSchema() + "ConnectionString").Value);
                }
            }
            // get the handler mappings

            // get the default documents
            if (element.Element(GetSchema() + "DefaultDocuments") != null)
            {
                Config.DefaultDocuments = new List<string>();
                foreach (var documentElement in element.Element(GetSchema() + "DefaultDocuments").
                    Elements(serialisationNamespace + "string"))
                {
                    Config.DefaultDocuments.Add(documentElement.Value);
                }
            }

            CommandResponse = Config;
            #endregion
        }

        #endregion
    }
}