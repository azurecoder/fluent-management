using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public class Website
    {
        /// <summary>
        /// A list of hostnames used by the website
        /// </summary>
        public List<string> Hostname { get; set; }
        /// <summary>
        /// The name of th website
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether the website is enabled or not
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// What the Mode is of the website - Free, Shared, Reserved
        /// </summary>
        public ComputeMode ComputeMode { get; set; }
        /// <summary>
        /// The current state of the website stopped/started
        /// </summary>
        public WebsiteState State { get; set; }
        /// <summary>
        /// The current usage of the website e.g. Normal
        /// </summary>
        public WebsiteUsage Usage { get; set; }
        /// <summary>
        /// The params that the website has been set up with 
        /// </summary>
        public WebsiteParameters WebsiteParameters { get; set; }
        /// <summary>
        /// The config for the website
        /// </summary>
        public WebsiteConfig Config { get; set; }
        /// <summary>
        /// Which web location this is in e.g. northeuropewebspace
        /// </summary>
        public string Webspace { get; set; }
        /// <summary>
        /// to set the website in North Europe
        /// </summary>
        public const string NorthEuropeWebSpace = "northeuropewebspace";
        /// <summary>
        /// To set the website in West Europe
        /// </summary>
        public const string WestEuropeWebSpace = "westeuropewebspace";

    }

}