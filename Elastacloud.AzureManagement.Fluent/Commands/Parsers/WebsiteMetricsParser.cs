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
    internal class WebsiteMetricsParser : BaseParser
    {
        /// <summary>
        /// Used to construct a website parser
        /// </summary>
        /// <param name="document"></param>
        public WebsiteMetricsParser(XDocument document)
            : base(document)
        {
            CommandResponse = new List<WebsiteMetric>();
        }

        /// <summary>
        /// Contains a collections of websites
        /// </summary>
        public List<WebsiteMetric> WebsiteMetrics { get; private set; }
        /// <summary>
        /// The namespace that contains details in an array string value
        /// </summary>
        public const string SerialisationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return "MetricResponses"; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        internal override void Parse()
        {
            if (WebsiteMetrics == null)
                WebsiteMetrics = new List<WebsiteMetric>();

            XNamespace serialisationNamespace = SerialisationNamespace;
            XElement rootElements = Document.Element(GetSchema() + RootElement);
            foreach (var element in rootElements.Elements(GetSchema() + "MetricResponse"))
            {
                var site = new WebsiteMetric();

                var data = element.Element(GetSchema() + "Data");

                // get the compute modefrom the instance 
                if (data.Element(GetSchema() + "DisplayName") != null)
                {
                    site.DisplayName = data.Element(GetSchema() + "DisplayName").Value;
                }
                // get the name of the site as in xxxx.azurewebsites.net
                if (data.Element(GetSchema() + "Name") != null)
                {
                    site.Name = data.Element(GetSchema() + "Name").Value;
                }
                // the start time of the samples collection
                if (data.Element(GetSchema() + "StartTime") != null)
                {
                    site.StartTime = DateTime.Parse(data.Element(GetSchema() + "StartTime").Value);
                }
                // the end time of the samples collection
                if (data.Element(GetSchema() + "EndTime") != null)
                {
                    site.EndTime = DateTime.Parse(data.Element(GetSchema() + "EndTime").Value);
                }
                // the end time of the samples collection
                if (data.Element(GetSchema() + "Unit") != null)
                {
                    site.Units = data.Element(GetSchema() + "Unit").Value;
                }
                // the units of the samples collection
                var total = data.Element(GetSchema() + "Values").Elements(GetSchema() + "MetricSample");

                if (total.FirstOrDefault() != null)
                {
                    // for the time being just take the first node as there isn't any need to enumerate the collection as this should contain one node
                    site.Total = int.Parse(total.FirstOrDefault().Element(GetSchema() + "Total").Value);
                }
                WebsiteMetrics.Add(site);
            }
           
            CommandResponse = WebsiteMetrics;
        }

        #endregion
    }
}