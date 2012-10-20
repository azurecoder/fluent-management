/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.Storage
{
    public interface ICertificateActivity
    {
        IStorageActivity AddCertificate(X509Certificate2 certificate);
        IStorageActivity AddPublishSettings(string path);
        IStorageActivity AddCertificateFromStore(string thumbprint);
    }
}