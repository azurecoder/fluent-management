namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// Used to complete an activity so that a transaction interface can be returned
    /// </summary>
    public interface IServiceCompleteActivity
    {
        /// <summary>
        /// Executes the command cascade
        /// </summary>
        IServiceTransaction Go();
    }
}