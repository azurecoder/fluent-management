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
    public enum RoleInstanceStatus
    {
        RoleStateUnknown,
// ReSharper disable InconsistentNaming
        CreatingVM,
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        StartingVM,
// ReSharper restore InconsistentNaming
        CreatingRole,
        StartingRole,
        ReadyRole,
        BusyRole,
        StoppingRole,
// ReSharper disable InconsistentNaming
        StoppingVM,
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        DeletingVM,
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        StoppedVM,
// ReSharper restore InconsistentNaming
        RestartingRole,
        CyclingRole,
        FailedStartingRole,
// ReSharper disable InconsistentNaming
        FailedStartingVM,
// ReSharper restore InconsistentNaming
        UnresponsiveRole 

    }
}