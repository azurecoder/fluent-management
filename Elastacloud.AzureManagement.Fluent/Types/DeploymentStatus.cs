/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// The status that a role instance can take
    /// </summary>
    public enum DeploymentStatus
    {
        RoleStateUnknown,
        CreatingVM,
        StartingVM,
        CreatingRole,
        StartingRole,
        ReadyRole,
        BusyRole,
        StoppingRole,
        StoppingVM,
        DeletingVM,
        StoppedVM,
        RestartingRole,
        CyclingRole,
        FailedStartingVM,
        UnresponsiveRole
    }
}