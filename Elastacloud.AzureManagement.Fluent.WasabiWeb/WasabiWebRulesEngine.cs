using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    public class WasabiWebRulesEngine : IWasabiWebRulesEngine 
    {
        private readonly List<IWasabiWebRule> _rules = new List<IWasabiWebRule>();

        public WasabiWebRulesEngine(string websiteName, int samplePeriod)
        {
            SamplesPeriodInMins = samplePeriod;
            WebsiteName = websiteName;
        }

        #region Implementation of IWasabiWebRulesEngine

        /// <summary>
        /// Adds a rule to the 
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(IWasabiWebRule rule)
        {
            bool exists = _rules.Exists(a => a.MetricName == rule.MetricName);
            if(exists)
                throw new WasabiWebException("duplicate detected - rule already exists for that particular metric");

            _rules.Add(rule);
        }

        /// <summary>
        /// A scalar used to determine whether the site should be scaled up down or left alone
        /// </summary>
        /// <returns>Whether the site should be scaled up or down</returns>
        public WasabiWebState Scale(WasabiWebLogicalOperation operation, List<WebsiteMetric> metrics)
        {
            int countScaleDown = 0, countScaleUp = 0;

            // enumerate the rules to determine the usage
            foreach (var rule in _rules)
            {
                // find the metric 
                var metric = metrics.Find(a => a.DisplayName == rule.MetricName);
                // for the time being if it doesn't exist just continue 
                if(metric == null)
                    continue;
                // if the value is greater than the total then scaleup count
                if (metric.Total > rule.IsGreaterThan)
                    countScaleUp++;
                // if the value is less than the total then scale down count
                if (metric.Total < rule.IsLessThan)
                    countScaleDown++;
            }
            // check to seee whether they should scale up or down
            if (countScaleDown == _rules.Count)
                return WasabiWebState.ScaleDown;
            // check directly to see whether the rules matches the count
            if (countScaleUp == _rules.Count)
                return WasabiWebState.ScaleUp;
            // if the logical operation is and but the count is not zero for either of them then fail
            if (WasabiWebLogicalOperation.And == operation && (countScaleDown != 0 || countScaleUp != 0))
            {
                return WasabiWebState.LeaveUnchanged;
            }
            // if the logical operation is Or then do the greater of the two
            if (WasabiWebLogicalOperation.Or == operation)
            {
                if(countScaleUp > countScaleDown)
                    return WasabiWebState.ScaleUp;
                if(countScaleDown > countScaleUp)
                    return WasabiWebState.ScaleDown;
            }

            return WasabiWebState.LeaveUnchanged;
        }

        /// <summary>
        /// Gets or sets the samples period in minutes 
        /// </summary>
        public int SamplesPeriodInMins { get; set; }

        /// <summary>
        /// The name of the website in WAWS
        /// </summary>
        public string WebsiteName { get; set; }

        #endregion
    }
}
