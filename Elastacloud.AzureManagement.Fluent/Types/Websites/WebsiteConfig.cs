/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public class WebsiteConfig
    {
        /// <summary>
        /// Creates a new instance of website config with empty collections
        /// </summary>
        public WebsiteConfig()
        {
            AppSettings = new Dictionary<string, string>();
            ConnectionStrings = new List<ConnStringInfo>();
            HandlerMappings = new Dictionary<string, string>();
            Metadata = new Dictionary<string, string>();
            DefaultDocuments = new List<string>();
        }
        /// <summary>
        /// the appsettings that are found in the web.config
        /// </summary>
        public Dictionary<string, string> AppSettings { get; set; }
        /// <summary>
        /// The connectionstrings that are found int he web.config
        /// </summary>
        public List<ConnStringInfo> ConnectionStrings { get; set; }
        /// <summary>
        /// Any supplementary metadata that is found in the site
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// the list of default documents
        /// </summary>
        public List<string> DefaultDocuments { get; set; }
        /// <summary>
        /// Whether error logging has been enabled for the site
        /// </summary>
        public bool DetailedErrorLoggingEnabled { get; set; }
        /// <summary>
        /// The list of handler mappings for the website e.g. python, ruby etc.
        /// </summary>
        public Dictionary<string, string> HandlerMappings { get; set; }
        /// <summary>
        /// A flag to denote whether the web server logs are available
        /// </summary>
        public bool HttpLoggingEnabled { get; set; }
        /// <summary>
        /// Which .NET framework is being used - 3.5/4.5
        /// </summary>
        public string NetFrameworkVersion { get; set; }
        /// <summary>
        /// Number of workers used
        /// </summary>
        public int NumberOfWorkers { get; set; }
        /// <summary>
        /// The PHP version used 5.3/5.4
        /// </summary>
        public string PhpVersion { get; set; }
        /// <summary>
        /// The scm and ftp publishing passwords
        /// </summary>
        public string PublishingPassword { get; set; }
        /// <summary>
        /// The scm publishing usernames
        /// </summary>
        public string PublishingUsername { get; set; }
        /// <summary>
        /// Whether web server logging is enabled for request tracing
        /// </summary>
        public bool RequestTracingEnabled { get; set; }
        /// <summary>
        /// 32-bit compatibility mode
        /// </summary>
        public bool Use32BitWorkerProcess { get; set; }
        /// <summary>
        /// The type of sourse control used
        /// </summary>
        public ScmType ScmType { get; set; }
    }
}