/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Commands.Services;

namespace Elastacloud.AzureManagement.Fluent.Commands.Storage
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class DeleteStorageAccountCommand : ServiceCommand
    {
        /// <summary>
        /// Given a storage service account delete the account with the given name
        /// </summary>
        internal DeleteStorageAccountCommand(string name)
        {
            Name = name;
            HttpVerb = HttpVerbDelete;
            ServiceType = "services";
            OperationId = "storageservices/" + name;
        }
    }
}