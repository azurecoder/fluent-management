/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    /// <summary>
    /// Used to return information about why the query is invalid and has failed
    /// </summary>
    internal class InvalidQueryException : ApplicationException
    {
        /// <summary>
        /// Used to construct an InvalidQueryException
        /// </summary>
        /// <param name="message">A message for the exception</param>
        public InvalidQueryException(string message) : base(message)
        {
        }

        /// <summary>
        /// Used to return the message information for the exception
        /// </summary>
        public override string Message
        {
            get
            {
                return "Linq query invalid: " + base.Message;
            }
        }
    }
}