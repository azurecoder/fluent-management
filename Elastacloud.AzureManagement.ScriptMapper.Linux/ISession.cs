using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        string PathToPrivateKey { get; set; }
        /// <summary>
        /// Creates a linux ssh session
        /// </summary>
        void CreateSession();
    }
}
