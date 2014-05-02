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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Used to manage the copying and registering of images within a subscription or intra-subscription
    /// </summary>
    public class ImageManagementClient : GenerateEventClientBase, IImageManagementClient
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
        public void CopyAndRegisterImageInNewSubscription(ImageProperties imageProperties, bool copyImageOnlyIfNotExists = true)
        {
            // by default we won't copy the image if it exists
            // TODO: need to check this implementation as the index is confusing 
            int index = imageProperties.Version;
            if (Exists(GetFormattedImageName(imageProperties.ImageNameRoot, imageProperties.Version, false)) && copyImageOnlyIfNotExists)
            {
                return;
            }
            RaiseClientUpdate(5, "Checked for formatted image existence");
            // get the storage account to copy to and the blob 
            var storageAccountClient = new StorageClient(SubscriptionId, ManagementCertificate);
            var destinationAccountKeys = storageAccountClient.GetStorageAccountKeys(imageProperties.DestinationAccountName);
            var storageAccount = new CloudStorageAccount(new StorageCredentials(imageProperties.DestinationAccountName, destinationAccountKeys[0]), true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            // list all of the containers in the blob - if they are not present then create a new one
            // create this container if it doesn't exist as this will contain the blob which will be registered as the image
            var containerReference = blobClient.GetContainerReference(imageProperties.DestinationAccountContainer);
            containerReference.CreateIfNotExists();
            // make sure that this image name dis a .vhd
            //imageName = imageName.EndsWith(".vhd") ? imageName + ".vhd" : imageName;
            var blobImage = containerReference.GetPageBlobReference(GetFormattedImageName(imageProperties.ImageNameRoot, index, true));
            while (blobImage.Exists())
            {
                // eventually we'll find a name we don't have! 
                blobImage = containerReference.GetPageBlobReference(GetFormattedImageName(imageProperties.ImageNameRoot, ++index, true));
            }
            RaiseClientUpdate(8, "Checked to see whether images exist with index " + index);
            // create a SAS from the source account for the image
            if (imageProperties.SourceAccountName != null && imageProperties.SourceAccountKey != null)
            {
                var client = new StorageClient(imageProperties.SourceAccountName, imageProperties.SourceAccountKey);
                imageProperties.ImageCopyLocation = client.GetSaSFromBlobUri(imageProperties.ImageCopyLocation);
                RaiseClientUpdate(10, "Calculated SaS blob uri");
            }
            // use the copy blob API to copy the image across 
            blobImage.StartCopyFromBlob(new Uri(imageProperties.ImageCopyLocation));
            double percentCopied = 0;
            while (blobImage.CopyState.Status != CopyStatus.Success && blobImage.CopyState.Status != CopyStatus.Failed)
            {
                blobImage = (CloudPageBlob)containerReference.GetBlobReferenceFromServer(GetFormattedImageName(imageProperties.ImageNameRoot, index, true));

                if (blobImage.CopyState.BytesCopied == null || blobImage.CopyState.TotalBytes == null)
                    continue;
                // wait one second until we have the copy status working properly
                double innerPercent = Math.Round(((double) blobImage.CopyState.BytesCopied.Value/(double) blobImage.CopyState.TotalBytes.Value) * 70) + 10;
                if (innerPercent != percentCopied)
                {
                    RaiseClientUpdate(Convert.ToInt32(innerPercent), "Copied part of image file to blob storage");
                }
                percentCopied = innerPercent;
                Thread.Sleep(1000);
            }
            
            // when the copy process is complete we want to register the image
            imageProperties.Name = imageProperties.Label = GetFormattedImageName(imageProperties.ImageNameRoot, index, false);
            imageProperties.MediaLink = blobImage.Uri.ToString();
            var registerImageCommand = new RegisterImageCommand(imageProperties)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            registerImageCommand.Execute();
            RaiseClientUpdate(100, "Completed registration of image into target account");
        }

        /// <summary>
        /// Gets a list of all of the available images in the subscription
        /// </summary>
        public List<ImageProperties> ImageList
        {
            get
            {
                var listImagesCommand = new ListImagesCommand()
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                listImagesCommand.Execute();
                return listImagesCommand.Properties;
            }
            
        }

        /// <summary>
        /// Checks whether a particular named image exists in the collection
        /// </summary>
        public bool Exists(string imageName)
        {
            return ImageList.Exists(properties => properties.Name == imageName || properties.Label == imageName);
        }

        private string GetFormattedImageName(string imageName, int index, bool withVhd)
        {
            return imageName + index + (withVhd ? ".vhd" : "");
        }

    }
}
