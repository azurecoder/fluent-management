using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    /// <summary>
    /// A class containing the data needed to access an azure account and pull back subscription and supplementary information
    /// </summary>
    public class LinqToAzureInputs
    {
        /// <summary>
        /// The subscription id used to query the SM API 
        /// </summary>
        public string SubscriptionId { get; set; }
        /// <summary>
        /// The thumbprint of the management cert used to pull back the details of the service
        /// </summary>
        /// <remarks>The certificate should be pre-installed - currently the linq API doesn't support loading from a .publishsettings</remarks>
        public string ManagementCertificateThumbprint { get; set; }
    }
}
