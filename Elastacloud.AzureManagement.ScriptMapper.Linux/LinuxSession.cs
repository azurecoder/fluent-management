/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    /// <summary>
    /// Specifically a linux implementation of the 
    /// </summary>
    public class LinuxSession : ISession, ICommandShell, IDisposable
    {
        /// <summary>
        /// An ssh client which belongs to the cluster deployment
        /// </summary>
        private SshClient _sshclient = null;
        /// <summary>
        /// An sftp client which belongs to the cluster deployment
        /// </summary>
        private SftpClient _sftpClient = null;
        /// <summary>
        /// Gets or sets whether an item has been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Used to construct a linux session command
        /// </summary>
        public LinuxSession(string host, int port, string userName, string userPassword, bool disablePasswordLogin, string pathToPvkFile = null)
        {
            UserName = userName;
            Hostname = host;
            Port = port;
            UserPassword = userPassword;
            if (DisablePasswordLogin = disablePasswordLogin)
            {
                PathToPrivateKey = pathToPvkFile;
            }
            CreateSession();
        }

        #region Implementation of ISession

        /// <summary>
        /// The username of the ssh user /home/{user}
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// Whether the user should connect to the vm using keys or password
        /// </summary>
        public bool DisablePasswordLogin { get; set; }

        /// <summary>
        /// The path to the .pem file 
        /// </summary>
        public string PathToPrivateKey { get; set; }
        /// <summary>
        /// The working directory to put all files in
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The name of the host we're trying to connect with 
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The port that the hostname is connectable on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Creates a linux ssh session
        /// </summary>
        public void CreateSession()
        {
            //var keyConverter = new SSHKeyConverter(PathToPrivateKey, UserPassword);
            //if (!keyConverter.Convert())
            //{
            //    throw new ApplicationException("unable to convert key file");
            //}
            
            _sshclient = DisablePasswordLogin ? new SshClient(Hostname, Port, UserName, new PrivateKeyFile(PathToPrivateKey, UserPassword)) : new SshClient(Hostname, Port, UserName, UserPassword);
            _sshclient.Connect();

            _sftpClient = DisablePasswordLogin ? new SftpClient(Hostname, Port, UserName, new PrivateKeyFile(PathToPrivateKey, UserPassword)) : new SftpClient(Hostname, Port, UserName, UserPassword);
            _sftpClient.Connect();
        }

        /// <summary>
        /// Copies a directory to the local filesystem from the remote server
        /// </summary>
        /// <param name="remoteDirectory">The absolute path of the directory name</param>
        /// <param name="localDirectory">The absolute path of the directory name</param>
        public void CopyDirectoryLocally(string remoteDirectory, string localDirectory)
        {
            _sftpClient.ChangeDirectory(remoteDirectory);
            var files = _sftpClient.ListDirectory(remoteDirectory);
            foreach (var sftpFile in files)
            {
                CopyFileLocally(sftpFile.FullName, sftpFile.Name, localDirectory);   
            }
        }

        /// <summary>
        /// A method to copy the remote file to the local file
        /// </summary>
        /// <param name="remoteFilePath">The remote file on the server</param>
        /// /// <param name="remoteFileName">The name of the remote file</param>
        /// <param name="localDirectory">The local directoryto place the file</param>
        public void CopyFileLocally(string remoteFilePath, string remoteFileName, string localDirectory)
        {
            var stream = new FileStream(Path.Combine(localDirectory, remoteFileName), FileMode.Create, FileAccess.Write);
            string remoteFileFullName = !remoteFilePath.EndsWith("/")
                                        ? String.Format("{0}{1}", remoteFilePath, remoteFileName)
                                        : String.Format("{0}/{1}", remoteFilePath, remoteFileName);
            _sftpClient.DownloadFile(remoteFileFullName, stream);
        }

        /// <summary>
        /// uploads a file to the a directory of the server 
        /// </summary>
        /// <param name="remotePath">The absolute remote path of the file - it will be overwritten every time</param>
        /// <param name="localPath">the local path from which the file be uploaded</param>
        public void UploadFile(string remotePath, string localPath)
        {
            _sftpClient.UploadFile(new FileStream(localPath, FileMode.Open, FileAccess.Read), remotePath);
        }

        #endregion 

        #region Implementation of ICommandShell

        /// <summary>
        /// Executes a command given a string input in the current context of the user
        /// </summary>
        public string ExecuteCommand(string command)
        {
            var sshCommand = _sshclient.CreateCommand(command);
            return sshCommand.Execute();
        }

        public string ExecuteWithRetries(string command, int count)
        {
            string response = null;
            int i = 0;
            while (String.IsNullOrEmpty(response) && i++ <= count)
            {
                response = ExecuteCommand(command);
            }
            return response;
        }

        /// <summary>
        /// Executes a shell command in the context of a sudo su
        /// </summary>
        public async Task<string> ExecuteShell(List<string> commands)
        {
            Task<string> task;
            var stream = new MemoryStream(5000);
            var shell = _sshclient.CreateShell(Encoding.Default, String.Join(@"\r\n", commands), stream, null);
            shell.Start();
            using (var reader = new StreamReader(stream))
            {
                task = reader.ReadLineAsync();
            }
            return await task;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _sshclient.Disconnect();
                _sftpClient.Disconnect();
                _sftpClient.Dispose();
                _sshclient.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}
