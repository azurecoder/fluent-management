/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Commands.SqlAzure
{
    public interface ISqlAzureFirewallRule
    {
        string SqlAzureServerName { get; set; }
        string SqlAzureRuleName { get; set; }
        string SqlAzureClientIpAddressHigh { get; set; }
        string SqlAzureClientIpAddressLow { get; set; }

        void ConfigureFirewallCommand(string server);
    }
}