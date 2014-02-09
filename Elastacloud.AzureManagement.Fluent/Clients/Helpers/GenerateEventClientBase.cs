using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    public class GenerateEventClientBase : IGenerateUpdateEvent
    {
        /// <summary>
        /// An event for the update event
        /// </summary>
        public event FluentClientEventHandler ClientUpdate;

        /// <summary>
        /// Raises a client update event 
        /// </summary>
        public void RaiseClientUpdate(int percentage, string description)
        {
            // percentage can't be more than 100%
            if (percentage > 100)
                percentage = 100;
            if(ClientUpdate != null)
                ClientUpdate(percentage, description);
        }
    }
}
