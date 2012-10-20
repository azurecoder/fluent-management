/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Net;
using System.Threading;

namespace Elastacloud.AzureManagement.Fluent
{
    /// <summary>
    /// The ServiceManager class sends a request to the management endpoint using the REST Management API in Azure
    /// </summary>
    public class ServiceManager
    {
        #region Delegates

        /// <summary>
        /// Used to propogate an exception thrown by Azure
        /// </summary>
        public delegate void AsyncResponseException(WebException exception);

        /// <summary>
        /// A delegate response parser 
        /// </summary>
        public delegate void AsyncResponseParser(HttpWebResponse response);

        #endregion

        /// <summary>
        /// Used to hold a mutex to signal a blocking call 
        /// </summary>
        internal readonly Semaphore Semaphore;

        /// <summary>
        /// Default constructor creates a fire and forget async query manager 
        /// </summary>
        public ServiceManager()
        {
        }

        /// <summary>
        /// Overloaded constructor used to aid releasing a blocking call to the Service Manager
        /// </summary>
        public ServiceManager(Semaphore semaphore)
            : this()
        {
            Semaphore = semaphore;
        }

        #region Properties

        /// <summary>
        /// Returns the default Xml schema for windows azure
        /// </summary>
        public const string DefaultWindowsAzureXmlNamespace = "http://schemas.microsoft.com/windowsazure";

        #endregion
    }
}