namespace Elastacloud.AzureManagement.Fluent.WasabiWeb
{
    /// <summary>
    /// Denotes the state in Wasabi 
    /// </summary>
    public enum WasabiWebState
    {
        /// <summary>
        /// Scales up a windows azure websites site
        /// </summary>
        ScaleUp,
        /// <summary>
        /// Leaves a website instance site
        /// </summary>
        LeaveUnchanged,
        /// <summary>
        /// Scales down a windows azure websites site
        /// </summary>
        ScaleDown
    }
}