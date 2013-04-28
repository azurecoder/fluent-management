/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    /// The Microsoft gallery supported images
    /// </summary>
    public enum VirtualMachineTemplates
    {
        /// <summary>
        /// The SQL Server 2012 image
        /// </summary>
        SqlServer2012Enterprise,
        SqlServer2012Web,
        SqlServer2012Standard,
// ReSharper disable InconsistentNaming
        WindowsServer2008R2SP1_30GB,
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        WindowsServer2008R2SP1_127GB,
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        WindowsServer2012_30GB,
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        WindowsServer2012_127GB,
// ReSharper restore InconsistentNaming
        BiztalkServer2013Standard,
        BiztalkServer2013Enterprise

    }
}