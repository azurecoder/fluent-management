/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent
{
    /// <summary>
    /// Interface used by each class to perform transactions based on a set of orchestration steps 
    /// </summary>
    public interface IServiceTransaction
    {
        /// <summary>
        /// Used to denote whether the transaction has been started or not
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// Used to denote whether the transaction has succeeded or not
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        /// Used to commit the transaction data 
        /// </summary>
        /// <returns>A dynamic type which represents the return of the particular transaction</returns>
        dynamic Commit();

        /// <summary>
        /// Used to rollback the transaction in the event of failure 
        /// </summary>
        void Rollback();
    }
}