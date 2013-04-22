using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// The interface used to roll out websites 
    /// </summary>
    public interface IWebsiteClient
    {
        /// <summary>
        /// Returns a list of websites
        /// </summary>
        List<Website> List();
        /// <summary>
        /// Creates a website given github credentials
        /// </summary>
        /// <param name="githubDetails">the details of the github repo</param>
        /// <param name="name">the site name </param>
        /// <param name="location">the site location</param>
        void CreateFromGithub(GithubDetails githubDetails, string name, string location = LocationConstants.NorthEurope);
        /// <summary>
        /// Creates a default website with nothing deployed
        /// </summary>
        /// <param name="website">the website which will be used</param>
        void Create(Website website);
        /// <summary>
        /// Deletes a website with the current context
        /// </summary>
        void Delete();
        /// <summary>
        /// Gets or sets the number of instances of the website
        /// </summary>
        int InstanceCount { get; set; }
        /// <summary>
        /// Gets or sets the compute of the site
        /// </summary>
        ComputeMode ComputeMode { get; set; }
        /// <summary>
        /// Restarts the website from a stopped state
        /// </summary>
        void Restart();
        /// <summary>
        /// Stops the website if it is currently running
        /// </summary>
        void Stop();
        /// <summary>
        /// The name of the website
        /// </summary>
        string Name { get; set; }
    }
}