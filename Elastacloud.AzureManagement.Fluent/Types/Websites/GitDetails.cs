using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// the details needed to bind to a remote github repository
    /// </summary>
    public class GitDetails
    {
        /// <summary>
        /// The username to login to github
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password used to login to github
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// The github repository name to use
        /// </summary>
        public string RepositoryName { get; set; }
    }
}