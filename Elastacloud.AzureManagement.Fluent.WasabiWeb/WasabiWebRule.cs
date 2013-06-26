using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// Concrete implementation of an IWasabiWebRule
    /// </summary>
    public class WasabiWebRule : IWasabiWebRule
    {
        /// <summary>
        /// Used to construct a WasabiWebRule
        /// </summary>
        public WasabiWebRule(string metricName, int isGreaterThan, int isLessThan)
        {
            MetricName = metricName;
            IsGreaterThan = isGreaterThan;
            IsLessThan = isLessThan;
        }

        #region Implementation of IWasabiWebRule

        /// <summary>
        /// A metric name used from a set of constants
        /// </summary>
        public string MetricName { get; set; }

        /// <summary>
        /// A value to denote that the rule passes if is greater than this value
        /// </summary>
        public int IsGreaterThan { get; set; }

        /// <summary>
        /// A value to denote that the rule passes if it is less than this value
        /// </summary>
        public int IsLessThan { get; set; }

        /// <summary>
        /// Tests the state of the website
        /// </summary>
        public WasabiWebState Test(int value)
        {
            if(value < IsLessThan)
                return WasabiWebState.ScaleDown;
            if(value > IsGreaterThan)
                return WasabiWebState.ScaleUp;
            return WasabiWebState.LeaveUnchanged;
        }

        #endregion
    }
}
