using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public interface IExecute
    {
        void Execute();
        void ParseTokens(string[] args);
    }
}