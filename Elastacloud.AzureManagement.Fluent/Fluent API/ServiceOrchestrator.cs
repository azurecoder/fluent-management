/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent
{
    /// <summary>
    /// A class used to orchestrate a series of steps in the process of deployment
    /// </summary>
    public class ServiceOrchestrator : IServiceTransaction
    {
        /// <summary>
        /// List containing the service transactions from the orchestration
        /// </summary>
        private readonly List<IServiceTransaction> _transactionList;

        /// <summary>
        /// Contains whether or not the transaction has been successful 
        /// </summary>
        private bool _success;

        /// <summary>
        /// Used to create a new list of IServiceTransaction orchestrated events
        /// </summary>
        public ServiceOrchestrator()
        {
            _transactionList = new List<IServiceTransaction>();
        }

        /// <summary>
        /// Adds a service transaction as a step to the deployment 
        /// </summary>
        /// <param name="transaction"></param>
        public void AddDeploymentStep(IServiceTransaction transaction)
        {
            _transactionList.Add(transaction);
        }

        #region Implementation of IServiceTransaction

        /// <summary>
        /// Used to commit the transaction data 
        /// </summary>
        /// <returns>A dynamic type which represents the return of the particular transaction</returns>
        public dynamic Commit()
        {
            _success = true;
            // TODO: Make this specific so that all of the parameters for a deployment are returned 
            foreach (IServiceTransaction serviceTransaction in _transactionList)
            {
                serviceTransaction.Commit();
                if (!serviceTransaction.Succeeded)
                {
                    _transactionList.ForEach(a =>
                                                 {
                                                     if (a.Started)
                                                     {
                                                         a.Rollback();
                                                     }
                                                 });
                    return (_success = false);
                }
            }

            return _success;
        }

        /// <summary>
        /// Used to rollback the transaction in the event of failure 
        /// </summary>
        public void Rollback()
        {
            foreach (IServiceTransaction serviceTransaction in _transactionList)
                serviceTransaction.Rollback();
        }

        /// <summary>
        /// Used to denote whether the transaction has been started or not
        /// </summary>
        public bool Started
        {
            get { return true; }
        }

        /// <summary>
        /// Used to denote whether the transaction has succeeded or not
        /// </summary>
        public bool Succeeded
        {
            get { return _success; }
        }

        #endregion
    }
}