/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// the details needed to bind to a remote github repository
    /// </summary>
    public enum ScmType
    {
        /// <summary>
        /// The local scm created by websites
        /// </summary>
        LocalGit,
        /// <summary>
        /// The remote github repo
        /// </summary>
        GitHub,
    }
}