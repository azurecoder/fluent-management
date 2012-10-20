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
    /// Used to delete a blob and snapshots within a specified container
    /// </summary>
    internal class DeleteBlobCommand : BlobCommand
    {
        internal DeleteBlobCommand(string containerName, string blobName)
        {
            ContainerName = containerName;
            BlobName = blobName;
            HttpVerb = HttpVerbDelete;
        }

        public override void Execute()
        {
            string accessContainer = String.Format("http://{0}.blob.core.windows.net/{1}/{2}", AccountName,
                                                   ContainerName, BlobName);
            string canResource = String.Format("/{0}/{1}/{2}", AccountName, ContainerName, BlobName);
            string authHeader = CreateAuthorizationHeader(canResource);
            SendWebRequest(accessContainer, authHeader);
        }
    }
}