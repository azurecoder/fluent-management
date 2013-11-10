using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    public class WebspaceProperties
    {
        public int InstanceCount { get; set; }
        public string Name { get; set; }
        public ComputeMode ComputeMode { get; set; }
    }
}
