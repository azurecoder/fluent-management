/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Commands.Services;

namespace Elastacloud.AzureManagement.Fluent.Commands
{
    /// <summary>
    /// Maps the type of each command to the string value of it's Azure name
    /// </summary>
    internal class CommandNameMap
    {
        /// <summary>
        /// The container for the command map to the string values
        /// </summary>
        internal static Dictionary<Type, string> CommandNameMapContainer = new Dictionary<Type, string>();

        /// <summary>
        /// Constructs a map with a list of predefined commands 
        /// </summary>
        static CommandNameMap()
        {
            CommandNameMapContainer.Add(typeof (GetAsyncStatusCommand), "GetOperationStatus");
        }

        /// <summary>
        /// Gets the name of the command from the list
        /// </summary>
        /// <param name="command">The ServiceCommand</param>
        /// <returns>The "name" of the command within the Service Management API</returns>
        public string GetCommandName(ServiceCommand command)
        {
            string commandType = CommandNameMapContainer[command.GetType()];
            if (commandType == null)
                return command.ToString();
            return commandType;
        }
    }
}