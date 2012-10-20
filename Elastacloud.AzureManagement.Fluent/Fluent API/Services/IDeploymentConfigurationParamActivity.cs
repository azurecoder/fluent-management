/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    public interface IDeploymentConfigurationParamActivity
    {
        IRoleReference AddParams(DeploymentParams? pParams);
        IDeploymentConfigurationParamActivity AddEnvironment(DeploymentSlot slot);
        IDeploymentConfigurationParamActivity AddDescription(string description);
        IDeploymentConfigurationParamActivity AddLocation(string location);
        void GoHostedServiceDeployment();
    }

    [Flags]
    public enum DeploymentParams
    {
        WarningsAsErrors = 1,
        StartImmediately = 2
    }
}