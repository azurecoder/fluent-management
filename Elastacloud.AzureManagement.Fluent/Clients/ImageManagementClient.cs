/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/


using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class ImageManagementClient : IImageManagementClient
    {
        public ImageManagementClient(string subscriptionId, X509Certificate2 certificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
        }

        public X509Certificate2 ManagementCertificate { get; set; }

        public string SubscriptionId { get; set; }

        /// <summary>
        /// Used to copy or register an image from one subscription to another
        /// </summary>
        public void CopyAndRegisterImageInNewSubscription(string accountName, string accountKey, string containerName,
            string imageName,
            string imageUri)
        {
            // get the storage account to copy to and the blob 
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            // list all of the containers in the blob - if they are not present then create a new one
            // create this container if it doesn't exist as this will contain the blob which will be registered as the image
            var containerReference = blobClient.GetContainerReference(containerName);
            containerReference.CreateIfNotExists();
            // make sure that this image name dis a .vhd
            int index = 0;
            imageName = imageName.EndsWith(".vhd") ? imageName + ".vhd" : imageName;
            var blobImage = containerReference.GetBlockBlobReference(GetFormattedImageName(imageName, index, true));
            while (!blobImage.Exists())
            {
                // eventually we'll find a name we don't have! 
                blobImage = containerReference.GetBlockBlobReference(GetFormattedImageName(imageName, ++index, true));
            }
            // create a SAS from the source account for the image
            //var client = new StorageClient(SubscriptionId, ManagementCertificate);
            //client.
            // use the copy blob API to copy the image across 
            blobImage.StartCopyFromBlob(new Uri(imageUri));
            while (blobImage.CopyState.Status != CopyStatus.Success || blobImage.CopyState.Status != CopyStatus.Failed)
            {
                // wait one second until we have the copy status working properly
                Thread.Sleep(1000);
            }
            
            // when the copy process is complete we want to register the image
        }

        private string GetFormattedImageName(string imageName, int index, bool withVhd)
        {
            return imageName + index + (withVhd ? ".vhd" : "");
        }
    }
}
