namespace Elastacloud.AzureManagement.Fluent.SqlAzure
{
    public interface ISqlCompleteActivity
    {
        /// <summary>
        /// Executes the command to build the server and database
        /// </summary>
        /// <returns>An IServerTransaction interface</returns>
        IServiceTransaction Go();
    }
}