/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using FSharp.Data.Runtime;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class GetServiceBusNamespaceListCommand : ServiceCommand
    {
        internal GetServiceBusNamespaceListCommand(string location)
        {
            HttpVerb = HttpVerbGet;
            Location = location;
            ServiceType = "services";
            OperationId = "ServiceBus";
            HttpCommand = "Namespaces";
            ContentType = "application/xml;charset=utf-8";
        }

        public IEnumerable<string> Namespaces { get; private set; }
       

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            XNamespace netservices = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";
            XDocument document;

            var list = new List<string>();

            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                document = XDocument.Parse(reader.ReadToEnd());
            }
            var root = document.Descendants(netservices + "NamespaceDescription");
            root.ToList().ForEach(item =>
            {
                if (item.Element(netservices + "Region").Value == Location)
                {
                    list.Add(item.Element(netservices + "Name").Value);
                }
            });
            Namespaces = list;
            SitAndWait.Set();
        }
    }
}