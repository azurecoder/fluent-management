/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Storage
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class CreateStorageAccountCommand : ServiceCommand
    {
        internal CreateStorageAccountCommand(string name, string description, string location = "North Europe")
        {
            Name = name.ToLower();
            Description = description;
            Location = location;
            HttpVerb = HttpVerbPost;
            ServiceType = "services";
            OperationId = "storageservices";
        }

        protected override string CreateXmlPayload()
        {
            /* <?xml version="1.0" encoding="utf-8"?>
                <CreateStorageServiceInput xmlns="http://schemas.microsoft.com/windowsazure">
                   <ServiceName>service-name</ServiceName>
                   <Description>service-description</Description>
                   <Label>base64-encoded-label</Label>
                   <AffinityGroup>affinity-group-name</AffinityGroup>
                   <Location>location-of-the-storage-account</Location>
                </CreateStorageServiceInput>*/
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "CreateStorageServiceInput",
                             new XElement(ns + "ServiceName", Name),
                             new XElement(ns + "Description", Description),
                             new XElement(ns + "Label", Convert.ToBase64String(Encoding.UTF8.GetBytes(Name))),
                             new XElement(ns + "Location", Location)));
            return doc.ToStringFullXmlDeclaration();
        }
    }
}