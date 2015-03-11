/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Blobs
{
    /// <summary>
    /// Used to create a container within a specified storage services account
    /// </summary>
    internal class CreateBlobContainerCommand : BlobCommand
    {
        internal CreateBlobContainerCommand(string containerName, string defaultLocation = LocationConstants.NorthEurope)
        : base(defaultLocation)
        {
            ContainerName = containerName;
            HttpVerb = HttpVerbPut;
        }

        protected override StorageServiceType StorageServiceType
        {
            get { return StorageServiceType.Blob; }
        }

        public override void Execute()
        {
            string accessContainer = String.Format("http://{0}.blob.core.{1}/{2}?restype=container", AccountName,
                                                   Postfix, ContainerName);
            string canResource =
                String.Format("/{0}/{1}\nrestype:container", AccountName, ContainerName);
            string authHeader = CreateAuthorizationHeader(canResource);
            SendWebRequest(accessContainer, authHeader);
        }
    }
}