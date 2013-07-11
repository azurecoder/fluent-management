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
        VisualStudioUltimate2013Preview,
        BizTalkServer2013Enterprise,
        BizTalkServer2013Evaluation,
        BizTalkServer2013Standard,
        OpenLogicCentOS63,
        WindowsServer2008R2SP1May2013,
        WindowsServer2008R2SP1June2013,
        WindowsServer2012DatacenterMay2013,
        WindowsServer2012DatacenterJune2013,
        WindowsServer2012R2Preview,
        UbuntuServer12042LTS,
        UbuntuServer1210,
        UbuntuServer1304,
        SUSELinuxEnterpriseServer11ServicePack2,
// ReSharper disable InconsistentNaming
        openSUSE123,
// ReSharper restore InconsistentNaming
        SharePointServer2013Trial,
        SQLServer2008R2SP2EnterpriseOnWindowsServer2008R2SP1,
        SQLServer2008R2SP2StandardOnWindowsServer2008R2SP1,
        SQLServer2012SP1EnterpriseOnWindowsServer2008R2,
        SQLServer2012SP1EnterpriseOnWindowsServer2008R2SP1,
        SQLServer2012SP1EnterpriseOnWindowsServer2012,
        SQLServer2012SP1StandardOnWindowsServer2008R2SP1,
        SQLServer2012SP1StandardOnWindowsServer2012,
        SQLServer2012SP1WebOnWindowsServer2008R2SP1,
        SQLServer2014CTP1EvaluationEditionOnWindowsServer2012,
        SQLServer2014CTP1EvaluationEditionOnWindowsServer2012R2
    }
}