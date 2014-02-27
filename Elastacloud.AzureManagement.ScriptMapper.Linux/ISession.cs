/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.IO;

namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    /// <summary>
    /// A session is used to connect to the linux vm by ssh
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// The username of the ssh user /home/{user}
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// The password of the user
        /// </summary>
        string UserPassword { get; set; }
        /// <summary>
        /// Whether the user should connect to the vm using keys or password
        /// </summary>
        bool DisablePasswordLogin { get; set; }
        /// <summary>
        /// The path to the .pem file 
        /// </summary>
        Stream PrivateKey { get; set; }
        /// <summary>
        /// The name of the host we're trying to connect with 
        /// </summary>
        string Hostname { get; set; }
        /// <summary>
        /// The port that the hostname is connectable on
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Creates a linux ssh session
        /// </summary>
        void CreateSession();
        /// <summary>
        /// Copies a directory to the local filesystem from the remote server
        /// </summary>
        /// <param name="remoteDirectory">The absolute path of the directory name</param>
        /// <param name="localDirectory">The absolute path of the directory name</param>
        void CopyDirectoryLocally(string remoteDirectory, string localDirectory);

        /// <summary>
        /// A method to copy the remote file to the local file
        /// </summary>
        /// <param name="remoteFilePath">The remote file on the server</param>
        /// <param name="remoteFileName">The name of the remote file</param>
        /// <param name="localDirectory">The local directoryto place the file</param>
        void CopyFileLocally(string remoteFilePath, string remoteFileName, string localDirectory);
        /// <summary>
        /// uploads a file to the a directory of the server 
        /// </summary>
        /// <param name="remotePath">The absolute remote path of the file - it will be overwritten every time</param>
        /// <param name="localPath">the local path from which the file be uploaded</param>
        void UploadFile(string remotePath, string localPath);
    }
}
