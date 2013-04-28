/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Text.RegularExpressions;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    public class VmConstants
    {
        #region Biztalk Server 2013

        public const string VmTemplateBizTalkServer2013Enterprise = "2cdc6229df6344129ee553dd3499f0d3__BizTalk-Server-2013-Enterprise";
        public const string VmTemplateBizTalkServer2013Standard = "2cdc6229df6344129ee553dd3499f0d3__BizTalk-Server-2013-Standard";

        #endregion 

        #region Sql Server 2012

        public const string VmTemplateSqlServer2012Enterprise = "fb83b3509582419d99629ce476bcb5c8__Microsoft-SQL-Server-2012SP1-Enterprise-CY13SU04-SQL11-SP1-CU3-11.0.3350.0-B";
        public const string VmTemplateSqlServer2012Standard = "fb83b3509582419d99629ce476bcb5c8__Microsoft-SQL-Server-2012SP1-Standard-CY13SU04-SQL11-SP1-CU3-11.0.3350.0-B";
        public const string VmTemplateSqlServer2012Web = "fb83b3509582419d99629ce476bcb5c8__Microsoft-SQL-Server-2012SP1-Web-CY13SU04-SQL11-SP1-CU3-11.0.3350.0";

        #endregion

        #region Windows Server 2008/2012
        // ReSharper disable InconsistentNaming
        public const string VmTemplateWin2K8SP1_DataCentre_127GB = "a699494373c04fc0bc8f2bb1389d6106__Win2K8R2SP1-Datacenter-201303.01-en.us-127GB.vhd";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public const string VmTemplateWin2K8SP1_DataCentre_30GB = "a699494373c04fc0bc8f2bb1389d6106__Win2k8R2SP1-Datacenter-201302.01-en.us-30GB.vhd";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public const string VmTemplateWin2012_DataCentre_30GB = "a699494373c04fc0bc8f2bb1389d6106__Windows-Server-2012-Datacenter-201302.01-en.us-30GB.vhd";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public const string VmTemplateWin2012_DataCentre_127GB = "a699494373c04fc0bc8f2bb1389d6106__Windows-Server-2012-Datacenter-201304.01-en.us-127GB.vhd";
        // ReSharper restore InconsistentNaming
        #endregion 
    }
}