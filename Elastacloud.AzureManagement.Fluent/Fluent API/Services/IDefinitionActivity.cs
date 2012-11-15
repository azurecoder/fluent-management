using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Services;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    public interface IDefinitionActivity
    {
        /// <summary>
        /// Used to enable a role for SSL
        /// </summary>
        IServiceCertificate EnableSslForRole(string name);

        /// <summary>
        /// Used to enable remote desktop 
        /// </summary>
        IRemoteDesktop EnableRemoteDesktopForRole(string name);


        /// <summary>
        /// Used to enable remote desktop 
        /// </summary>
        IRemoteDesktop EnableRemoteDesktopAndSslForRole(string name);
    }
}
