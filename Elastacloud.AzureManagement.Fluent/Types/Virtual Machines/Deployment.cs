/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// This is used as the underlying wrapper for the deployment of the virtual machine
    /// </summary>
    public class Deployment : ICustomXmlSerializer
    {
        /// <summary>
        /// The name of the deployment as it stands currently
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The deployment label in Base 64 encoded text
        /// </summary>
        public string Label { get; set; }

        // TODO: Add a new node for Dns and VirtualNetworking name

        // TODO: Add the custom serialisation here with the namespace and return as a root node 
        /// <summary>
        /// The list of role that will be deployed in this deployment
        /// </summary>
        public RoleList RoleList { get; set; }

        /// <summary>
        /// Used to get a single deployment containing the SQL Server 2012 image
        /// </summary>
        /// <returns>A complete deployment of the SQL Server 2012 image</returns>
        public static Deployment GetDefaultSqlServer2012Deployment(string cloudServiceName, string storageAccount,
                                                                   VmSize vmSize = VmSize.Small)
        {
            return AddPersistentVMRole(cloudServiceName, PersistentVMRole.GetDefaultSqlServer2012VMRole(vmSize, storageAccount));
        }

        /// <summary>
        /// Used to create a deployment and add any persistent vm role to the deployment
        /// </summary>
        /// <param name="role">The PersistentVMRole</param>
        /// <param name="cloudServiceName">The Name of the cloud service which the role is present in</param>
        /// <returns>The Deployment that is being used</returns>
        private static Deployment AddPersistentVMRole(string cloudServiceName, PersistentVMRole role)
        {
            var namer = new RandomAccountName();
            var deployment = new Deployment
                                 {
                                     Name = cloudServiceName,
//                                     Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(cloudServiceName))
                                     Label = cloudServiceName
                                 };
            role.RoleName = namer.GetPureRandomValue();
            var roleList = new RoleList();
            roleList.Roles.Add(role);
            deployment.RoleList = roleList;
            return deployment;
        }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            return new XElement(Namespaces.NsWindowsAzure + "Deployment",
                                new XElement(Namespaces.NsWindowsAzure + "Name", Name),
                                new XElement(Namespaces.NsWindowsAzure + "DeploymentSlot", "Production"),
                                new XElement(Namespaces.NsWindowsAzure + "Label", Label), RoleList.GetXmlTree());
        }

        #endregion
    }
}