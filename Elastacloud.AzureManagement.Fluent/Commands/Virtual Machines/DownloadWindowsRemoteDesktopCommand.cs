/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Net;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    /// Creates a deployment for a virtual machine and allows some preconfigured defaults from the image gallery 
    /// </summary>
    internal class DownloadWindowsRemoteDesktopCommand : ServiceCommand
    {
        // GET https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/<deployment-name>/roleinstances/<roleinstance-name>/ModelFile?FileType=RDP
        /// <summary>
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        internal DownloadWindowsRemoteDesktopCommand(WindowsVirtualMachineProperties properties)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = properties.CloudServiceName + "/deployments/" + properties.DeploymentName + "/roleinstances/" + properties.RoleName + "/ModelFile?FileType=RDP";
            Properties = properties;
            HttpVerb = HttpVerbGet;
        }
        
        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public WindowsVirtualMachineProperties Properties { get; set; }

        /// <summary>
        /// Used to bring back and parse the context response for a virtual machine
        /// </summary>
        /// <param name="webResponse">the Http web response</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            var bytesInMem = new byte[500];
            var responseStream = webResponse.GetResponseStream();
            FileLength = responseStream.Read(bytesInMem, 0, 500);
            FileBytes = bytesInMem;
            // have to have some information on headers and other stuff 
            SitAndWait.Set();
        }

        /// <summary>
        /// Measures the length of the files returned by the request
        /// </summary>
        public int FileLength { get; set; }
        /// <summary>
        /// Returns the bytes of the RDP file 
        /// </summary>
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "DownloadWindowsRemoteDesktopCommand";
        }
    }
}