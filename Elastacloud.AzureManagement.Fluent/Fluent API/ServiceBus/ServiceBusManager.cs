using Elastacloud.AzureManagement.Fluent.Commands.ServiceBus;

namespace Elastacloud.AzureManagement.Fluent.ServiceBus
{
    public class ServiceBusManager
    {
        public ServiceBusManager(string @namespace, string key)
        {
            var command = new GetManagementTokenCommand(@namespace, key);
            command.Execute();
        }
    }
}