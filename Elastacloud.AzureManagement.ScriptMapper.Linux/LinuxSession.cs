using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    public class LinuxSession : ISession
    {
        private SshClient _sshclient = null;
        private SftpClient _sftpClient = null;

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
            _sftpClient.DownloadFile(remoteFilePath, stream);
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
    }
}
