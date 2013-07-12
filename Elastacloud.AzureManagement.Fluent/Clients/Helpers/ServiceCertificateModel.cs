using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    public class ServiceCertificateModel
    {
        public string Password { get; set; }
        public X509Certificate2 ServiceCertificate { get; set; }
    }
}
