namespace Elastacloud.AzureManagement.Fluent.Types.MobileServices
{
    /// <summary>
    /// The possible states for a mobile service
    /// </summary>
    public enum State
    {
        /// <summary>
        /// occurs when all of the resources are attached correctly
        /// </summary>
        Healthy,
        /// <summary>
        /// occurs when a database is missing
        /// </summary>
        UnHealthy
    }
}