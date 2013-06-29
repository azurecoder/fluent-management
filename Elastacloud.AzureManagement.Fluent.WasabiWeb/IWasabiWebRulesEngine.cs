using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    public interface IWasabiWebRulesEngine
    {
        /// <summary>
        /// Adds a rule to the 
        /// </summary>
        /// <param name="rule"></param>
        void AddRule(IWasabiWebRule rule);
        /// <summary>
        /// A scalar used to determine whether the site should be scaled up down or left alone
        /// </summary>
        /// <returns>Whether the site should be scaled up or down</returns>
        WasabiWebState Scale(WasabiWebLogicalOperation operation, List<WebsiteMetric> metrics);
        /// <summary>
        /// Gets or sets the samples period in minutes 
        /// </summary>
        int SamplesPeriodInMins { get; set; }
        /// <summary>
        /// The name of the website in WAWS
        /// </summary>
        string WebsiteName { get; set; }
        /// <summary>
        /// Indexer used to get the metric from a collection of metrics
        /// </summary>
        /// <param name="metricName">The name of the metric</param>
        /// <returns>An IWasabiWebRule interface containing details on the rule</returns>
        IWasabiWebRule this[string metricName] { get; }
    }
}