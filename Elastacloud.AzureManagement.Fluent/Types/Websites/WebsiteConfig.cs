using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public class WebsiteConfig
    {
        public Dictionary<string, string> AppSettings { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<string> DefaultDocuments { get; set; }
        public bool DetailedErrorLoggingEnabled { get; set; }
        public Dictionary<string, string> HandlerMappings { get; set; }
        public bool HttpLoggingEnabled { get; set; }
        public string NetFrameworkVersion { get; set; }
        public int NumberOfWorkers { get; set; }
        public string PhpVersion { get; set; }
        public string PublishingPassword { get; set; }
        public string PublishingUsername { get; set; }
        public bool RequestTracingEnabled { get; set; }
        public bool Use32BitWorkerProcess { get; set; }
    }

}