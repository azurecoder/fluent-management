using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// the details needed to bind to a remote github repository
    /// </summary>
 /************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
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