using NLog;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary>
    /// A class used for all logging events in the application
    /// </summary>
    public class ElastaLogger
    {
        /// <summary>
        /// Returns the default ElastaLogger class instance logger
        /// </summary>
        public static Logger MessageLogger = LogManager.GetCurrentClassLogger();
    }
}