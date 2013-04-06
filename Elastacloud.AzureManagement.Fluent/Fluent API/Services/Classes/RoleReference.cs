/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Linq;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    internal class RoleReference : IRoleReference, ICloudConfig
    {
        private readonly DeploymentManager _manager;

        internal RoleReference(DeploymentManager manager)
        {
            _manager = manager;
        }

        #region Implementation of IRoleReference

        IRoleActivity IRoleReference.ForRole(string name)
        {
            if (_manager.CscfgFileInstance == null)
                _manager.CscfgFileInstance = CscfgFile.GetInstance(_manager.LocalPackagePathName);
            var cscfg = CscfgFile.GetAdHocInstance(_manager.CscfgFileInstance.NewVersion);
            int count = cscfg.GetInstanceCountForRole(name);
           
            _manager.RolesInstances.Add(name, count);
            return _manager;
        }

        IRoleActivity IRoleReference.AndRole(string name)
        {
            if (_manager.RolesInstances.Count == 0)
                throw new ApplicationException("no roles have been added for explicit configuration use ForRole first");
            return ((IRoleReference) this).ForRole(name);
        }

        /// <summary>
        /// This is used to inject the configuration into the role and replace the existing configuration - it takes a filename param 
        /// </summary>
        IRoleReference IRoleReference.ReplaceConfiguration(string filename)
        {
            // this is inconsistent with the way that this is going to work - too many additions!!
            _manager.CscfgFileInstance = CscfgFile.GetInstance(filename);

            return _manager;
        }

        /// <summary>
        /// This is used to check whether all of the role instances are running in the deployment 
        /// </summary>
        IServiceCompleteActivity IRoleReference.WaitUntilAllRoleInstancesAreRunning()
        {
            _manager.WaitUntilAllRoleInstancesAreRunning = true;
            return _manager;
        }

        IServiceCompleteActivity IRoleReference.ReturnWithoutWaitingForRunningRoles()
        {
            _manager.WaitUntilAllRoleInstancesAreRunning = false;
            return _manager;
        }

        /// <summary>
        /// Executes the command cascade through a transaction interface
        /// </summary>
        /// <returns>An IServiceTransactions interface</returns>
        IServiceTransaction IServiceCompleteActivity.Go()
        {
            return new DeploymentTransaction(_manager);
        }

        #endregion

        #region Implementation of ICloudConfig

        /// <summary>
        /// This method updates the cloud config to ensure that all roles have their instance count updated
        /// </summary>
        XDocument ICloudConfig.ChangeConfig(XDocument document)
        {
            // TODO: this whole bit needs to be updated!!! It's beginning to look like a mess and can be replaced with 3-4 lines of code
            document.Declaration = new XDeclaration("1.0", "utf-8", "");
            foreach (string rn in _manager.RolesInstances.Keys)
            {
                int instanceCount = _manager.RolesInstances[rn];
                XElement role = document.Descendants(Namespaces.NsServiceManagement + "Role")
                    .Where(a => (string) a.Attribute("name") == rn)
                    .FirstOrDefault();
                // updates the instance count number here
                role.Elements(Namespaces.NsServiceManagement + "Instances")
                    .FirstOrDefault()
                    .Attribute("count").SetValue(instanceCount.ToString());
            }
            return document;
        }

        XDocument ICloudConfig.ChangeDefinition(XDocument document)
        {
            return document;
        }

        string ICloudConfig.Rolename
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}