using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class ApplicationFactory
    {
        public ApplicationFactory(string term, string operation, string subscriptionId, string publishSettings,
                                  string cloudServiceName, string deploymentName, string roleName, string password)
        {
            Term = term;
            Operation = operation;
            SubscriptionId = subscriptionId;
            CloudServiceName = cloudServiceName;
            RoleName = roleName;
            Password = password;
            DeploymentName = deploymentName;
            ManagementCertificate = GetCertificateFromPublishSettings(publishSettings);
            PublishSettingsRoot = Path.GetDirectoryName(publishSettings);
        }

        internal string DeploymentName { get; private set; }
        internal X509Certificate2 ManagementCertificate { get; private set; }
        internal string Password { get; private set; }
        internal string RoleName { get; private set; }
        internal string CloudServiceName { get; private set; }
        internal string SubscriptionId { get; private set; }
        internal string Operation { get; private set; }
        internal string Term { get; private set; }
        internal string PublishSettingsRoot { get; private set; }

        public IExecute GetExecutor()
        {
            if (Term == "mobile" && Operation == "create")
            {
                return new MobileCreate(this);
            }
            if (Term == "mobile" && Operation == "read")
            {
                return new MobileGet(this);
            }
            if (Term == "vm" && Operation == "create")
            {
                return new VmCreate(this);
            }
            if (Term == "vm" && Operation == "read")
            {
                return new VmGet(this);
            }
            if (Term == "vm" && Operation == "delete")
            {
                return new VmDelete(this);
            }
            if (Term == "site" && Operation == "read")
            {
                return new WebsiteGet(this);
            }
            if (Term == "site" && Operation == "create")
            {
                return new WebsiteCreate(this);
            }
            if (Term == "linux")
            {
                return new VmCreateLinux(this);
            }
            return null;
        }

        private X509Certificate2 GetCertificateFromPublishSettings(string publishSettings)
        {

            var settings = new PublishSettingsExtractor(publishSettings);
            
            return settings.AddPublishSettingsToPersonalMachineStore();
        }
    }
}
