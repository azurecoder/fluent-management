/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent
{
    public delegate void EventReached(EventPoint point, string message);

    public interface IAzureManager
    {
        string SubscriptionId { get; set; }
        X509Certificate2 ManagementCertificate { get; set; }
        event EventReached AzureTaskComplete;
    }

    public enum EventPoint
    {
        HostedServiceCreated,
        StorageAccountCreated,
        StorageAccountDeleted,
        StorageBlobContainerCreated,
        DeploymentPackageUploadComplete,
        DeploymentCreated,
        StorageKeyRequestSuccess,
        SqlAzureServerCreated,
        SqlAzureFirewallIpAdded,
        SqlAzureDatabaseCreated,
        SqlAzureDatabaseAdminCreated,
        SqlAzureScriptsExecutedSuccessfully,
        ExceptionOccurrence
    }
}