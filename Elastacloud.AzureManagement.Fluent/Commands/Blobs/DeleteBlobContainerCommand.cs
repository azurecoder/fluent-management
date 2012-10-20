/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;

namespace Elastacloud.AzureManagement.Fluent.Commands.Blobs
{
    /// <summary>
    /// Used to delete a blob container within a specified storage account
    /// </summary>
    internal class DeleteBlobContainerCommand : BlobCommand
    {
        internal DeleteBlobContainerCommand(string containerName)
        {
            ContainerName = containerName;
            HttpVerb = HttpVerbDelete;
        }

        public override void Execute()
        {
            string accessContainer = String.Format("http://{0}.blob.core.windows.net/{1}?restype=container", AccountName,
                                                   ContainerName);
            string canResource =
                String.Format("/{0}/{1}\nrestype:container", AccountName, ContainerName);
            string authHeader = CreateAuthorizationHeader(canResource);
            SendWebRequest(accessContainer, authHeader);
        }
    }
}