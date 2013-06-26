namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// The base interface for a rule
    /// </summary>
    public interface IWasabiWebRule
    {
        /// <summary>
        /// A metric name used from a set of constants
        /// </summary>
        string MetricName { get; set; }
        /// <summary>
        /// A value to denote that the rule passes if is greater than this value
        /// </summary>
        int IsGreaterThan { get; set; }
        /// <summary>
        /// A value to denote that the rule passes if it is less than this value
        /// </summary>
        int IsLessThan { get; set; }
        /// <summary>
        /// Tests the state of the website
        /// </summary>
        WasabiWebState Test(int value);
        /// <summary>
        /// Sets the value to test against
        /// </summary>
        int Value { get; set; }
    }
}
