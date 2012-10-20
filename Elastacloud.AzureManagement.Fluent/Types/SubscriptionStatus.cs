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
    /// Describes a Windows Azure subscription - can either be Disabled or Active
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// The account will be disabled if there are billing issues or a free account has transitioned into a paid account and requires 
        /// intervention
        /// </summary>
        Disabled,

        /// <summary>
        /// Normal status for an account
        /// </summary>
        Active
    }
}