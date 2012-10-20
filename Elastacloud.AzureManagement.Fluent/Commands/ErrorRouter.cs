/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Commands.Services;

namespace Elastacloud.AzureManagement.Fluent.Commands
{
    /// <summary>
    /// Used to route all kinds of exceptions to various part of the application through notifications and events
    /// </summary>
    internal class ErrorRouter
    {
        /// <summary>
        /// Builds up a dictionary of routing interests for the exception
        /// </summary>
        // TODO: Place a list 
        private static readonly Dictionary<string, int[]> CommandStatusMatches = new Dictionary<string, int[]>();

        /// <summary>
        /// The command being sent to the management API
        /// </summary>
        private readonly ServiceCommand _command;

        /// <summary>
        /// The Http Status code of the web exception
        /// </summary>
        private readonly int _statusCode;

        /// <summary>
        /// Populates the static collection from config
        /// </summary>
        // TODO: pre-populate this collection from configuration
        static ErrorRouter()
        {
            CommandStatusMatches.Add(new GetAsyncStatusCommand().ToString(), new[] {404});
        }

        /// <summary>
        /// Constructs a router for the type of command and exception that has been generated
        /// </summary>
        /// <param name="statusCode">An Http Status code between 100-500</param>
        /// <param name="command">The type of ServiceCommand being sent</param>
        public ErrorRouter(int statusCode, ServiceCommand command)
        {
            _command = command;
            _statusCode = statusCode;
        }

        /// <summary>
        /// Routes a message to a given notification interface
        /// </summary>
        public void Route()
        {
            //TODO: Place a notification interface here so that messages can be routed based on their type
        }
    }
}