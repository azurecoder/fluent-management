/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.IO;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    internal class DeploymentConfigurationFileActivity : IDeploymentConfigurationFileActivity
    {
        internal readonly DeploymentManager Manager;

        internal DeploymentConfigurationFileActivity(DeploymentManager manager)
        {
            Manager = manager;
        }

        #region Implementation of IDeploymentConfigurationFileActivity

        IDeploymentConfigurationStorageActivity IDeploymentConfigurationFileActivity.WithPackageConfigDirectory(string directoryName)
        {
            int cspkgCount = 0;
            Manager.DeploymentFolder = directoryName;

            if (!Directory.Exists(directoryName))
                throw new ApplicationException(String.Format("Directory {0} does not exist", directoryName));
            foreach (string fileName in Directory.EnumerateFiles(directoryName))
            {
                if (Path.GetExtension(fileName) == Constants.CspkgExtension)
                {
                    if (cspkgCount > 1)
                        throw new ApplicationException("Only a single .cspkg file can be present in the deployment folder");
                    Manager.LocalPackagePathName = fileName;
                    cspkgCount++;
                }
            }

            return Manager;
        }

        #endregion
    }
}