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
        /// Creates a linux ssh session
        /// </summary>
        public void CreateSession()
        {

        }

        #endregion
    }
}
