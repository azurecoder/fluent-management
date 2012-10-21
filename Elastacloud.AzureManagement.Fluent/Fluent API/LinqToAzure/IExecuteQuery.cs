/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.Linq;
using Elastacloud.AzureManagement.Fluent.Linq;

namespace Elastacloud.AzureManagement.Fluent.LinqToAzure
{
    /// <summary>
    /// Used to determine where to send the queries relating to which azure service based on type
    /// </summary>
    public interface IExecuteQuery
    {
        /// <summary>
        /// Executes the query performing the underlying operation
        /// </summary>
        /// <typeparam name="T">The type used to define the query - CloudService or StorageAccount</typeparam>
        /// <param name="inputs">The Authentication inputs needed to fulfil the operation</param>
        /// <returns>An IQueryable interface</returns>
        IQueryable<T> Execute<T>(LinqToAzureInputs inputs);
        /// <summary>
        /// Returns a valid type based on what should be executed in this context
        /// </summary>
        Type GetValidType();
    }
}