/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;

namespace Elastacloud.AzureManagement.Fluent.Types.Exceptions
{
    /// <summary>
    /// A special type of exception used to capture the command that failed
    /// </summary>
    public class FluentManagementException : ApplicationException
    {
        /// <summary>
        /// Used to construct a fluent management exceptino
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="commandName">The name of the command</param>
        public FluentManagementException(string message, string commandName) : base(commandName + ": " + message)
        {
            CommandName = commandName;
        }

        /// <summary>
        /// Used to capture the name of the command that failed
        /// </summary>
        public string CommandName { get; set; }
    }
}
