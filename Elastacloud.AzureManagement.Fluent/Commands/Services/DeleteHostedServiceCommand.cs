/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Net;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to delete a hosted service given a service name
    /// </summary>
    internal class DeleteHostedServiceCommand : ServiceCommand
    {
        /// <summary>
        /// Used to a construct a DeleteHostedService command
        /// </summary>
        /// <param name="name">The name of the hosted service</param>
        internal DeleteHostedServiceCommand(string name)
        {
            Name = name;
            HttpVerb = HttpVerbDelete;
            HttpCommand = Name;
            ServiceType = "services";
            OperationId = "hostedservices";
        }

        /// <summary>
        /// This command should be used synchronously 
        /// </summary>
        /// <param name="webResponse">The HttpWebResponse return</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            // TODO: Look at building a synchronous and asynchronous command intermediate layer
            SitAndWait.Set();
        }
    }
}