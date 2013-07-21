using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    public class LinuxSession : ISession
    {
        private SshClient _sshclient = null;
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
        }

        /// <summary>
        /// Copies a directory to the local filesystem from the remote server
        /// </summary>
        /// <param name="directoryName">The absolute path of the directory name</param>
        public void CopyDirectoryLocally(string directoryName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
