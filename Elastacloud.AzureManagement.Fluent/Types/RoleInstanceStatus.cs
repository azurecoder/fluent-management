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