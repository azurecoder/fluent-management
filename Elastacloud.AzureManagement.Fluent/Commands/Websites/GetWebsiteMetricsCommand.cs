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
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Websites
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class GetWebsiteMetricsCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        /*
         * GET
         * https://management.core.windows.net:8443/<Subscription-Id>/services/WebSpaces/<WebSpaceName>/sites/<SiteName>/metrics?names=<MetricName>,<MetricName>[...],&StartTime=<2013-01-15T00:00:00>&EndTime=<2013-01-16T12:00:00>
         */ 
        internal GetWebsiteMetricsCommand(Website website, DateTime startTime, DateTime endTime)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            Website = website;
            HttpVerb = HttpVerbGet;
            IsManagement = true;

            string startTimeText = startTime.ToString(@"yyyy-MM-ddTHH:mm");
            string endTimeText = endTime.ToString(@"yyyy-MM-ddTHH:mm");
           
            HttpCommand = String.Format("{0}/sites/{1}/metrics?StartTime={2}&EndTime={3}", Website.Webspace, Website.Name, startTimeText, endTimeText);
        }
        /// <summary>
        /// The website details including all of the information that comes back on the webspace
        /// </summary>
        public Website Website { get; set; }
        /// <summary>
        /// The metrics for a website
        /// </summary>
        public List<WebsiteMetric> WebsiteMetrics { get; set; }

        /// <summary>
        /// Used to populate the website regions
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            WebsiteMetrics = Parse(webResponse, BaseParser.WebsiteListParser, new WebsiteMetricsParser(null));
            SitAndWait.Set();
        }
    }
}