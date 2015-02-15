/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
	/// <summary>
	/// Used to create a hosted service within a given subscription
	/// </summary>
	internal class DeleteServiceBusNamespaceCommand : ServiceCommand
	{
        internal DeleteServiceBusNamespaceCommand(string name)
		{
            Name = name;
			HttpVerb = HttpVerbDelete;
			ServiceType = "services";
			OperationId = "ServiceBus";
            HttpCommand = "Namespaces/" + name;
            ContentType = "application/atom+xml;type=entry;charset=utf-8";
		}

		

        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            if (webResponse.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new FluentManagementException("delete service bus namespace not deleted", "DeleteServiceBusNamespaceCommand");
            }
            SitAndWait.Set();
        }
	}
}