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
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

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
        /// Gets an ad-hoc deployment for a Windows templated VM instance
        /// </summary>
        /// <param name="properties">The VM properties touse for the deployment</param>
        /// <returns>A valid deployment for the command</returns>
        public static Deployment GetAdHocWindowsTemplateDeployment(WindowsVirtualMachineProperties properties)
        {
            return AddPersistentVMRole(properties, new [] {PersistentVMRole.AddAdhocWindowsRoleTemplate(properties)});
        }

        /// <summary>
        /// Gets an ad-hoc deployment for a Windows templated VM instance
        /// </summary>
        /// <param name="properties">The VM properties touse for the deployment</param>
        /// <returns>A valid deployment for the command</returns>
        public static Deployment GetAdHocLinuxTemplateDeployment(List<LinuxVirtualMachineProperties> properties)
        {
            return AddPersistentVMRole(properties[0], PersistentVMRole.AddAdhocLinuxRoleTemplates(properties) );
        }

        /// <summary>
        /// Used to create a deployment and add any persistent vm role to the deployment
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="roles">The PersistentVMRole</param>
        /// <returns>The Deployment that is being used</returns>
        private static Deployment AddPersistentVMRole(VirtualMachineProperties properties, IEnumerable<PersistentVMRole> roles)
        {
            var namer = new RandomAccountName();
            var deployment = new Deployment
                                 {
                                     // use the first deployment property if it's not the same then fluent doesn't supporting deployment splitting at this level!
                                     Name = properties.DeploymentName,
//                                     Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(cloudServiceName))
                                     Label = properties.DeploymentName
                                 };
            var roleList = new RoleList();

            foreach (var role in roles)
            {
                role.RoleName = role.RoleName ?? namer.GetPureRandomValue();
                roleList.Roles.Add(role);
            }

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