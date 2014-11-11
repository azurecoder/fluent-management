using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks
{
    public class VirtualNetworkSite
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Id { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public List<Subnet> Subnets { get; set; }
    }

    public class Subnet
    {
        public string Name { get; set; }
        public string CidrAddressRange { get; set; }
    }
}
