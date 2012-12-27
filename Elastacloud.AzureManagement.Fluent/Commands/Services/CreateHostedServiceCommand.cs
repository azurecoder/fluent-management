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
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class CreateHostedServiceCommand : ServiceCommand
    {
        internal CreateHostedServiceCommand(string name, string description, string location = "North Europe")
        {
            Name = name;
            Description = description;
            Location = location;
            HttpVerb = HttpVerbPost;
            ServiceType = "services";
            OperationId = "hostedservices";
        }

        protected override string CreatePayload()
        {
            /*<?xml version="1.0" encoding="utf-8"?>
                <CreateHostedService xmlns="http://schemas.microsoft.com/windowsazure">
                    <ServiceName>service-name</ServiceName>
                    <Label>base64-encoded-service-label</Label>
                    <Description>description</Description>
                    <Location>location</Location>
                    <AffinityGroup>affinity-group</AffinityGroup>
                </CreateHostedService>*/
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "CreateHostedService",
                             new XElement(ns + "ServiceName", Name),
                             new XElement(ns + "Label", Convert.ToBase64String(Encoding.UTF8.GetBytes(Name))),
                             new XElement(ns + "Description", Description),
                             new XElement(ns + "Location", Location)));
            return doc.ToStringFullXmlDeclaration();
        }
    }
}