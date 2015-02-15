/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class CheckServiceBusNamespaceAvailabilityCommand : ServiceCommand
    {
        internal CheckServiceBusNamespaceAvailabilityCommand(string name)
        {
            Name = name;
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "ServiceBus";
            HttpCommand = "CheckNamespaceAvailability/?namespace=" + name;
            ContentType = "application/xml;charset=utf-8";
        }

        public bool IsAvailable { get; private set; }
        /* <?xml version="1.0" encoding="utf-8"?>
            <entry xmlns="http://www.w3.org/2005/Atom">
	            <id>uuid:13ee3e35-5ecf-4ef8-b9c6-4f76150785f9;id=11471985</id>
	            <title type="text"/>
	            <updated>2015-02-14T18:37:21Z</updated>
	            <content type="application/xml">
		            <NamespaceAvailability xmlns="http://schemas.microsoft.com/netservices/2010/10/servicebus/connect" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
			            <Result>true</Result>
			            <ReasonDetail i:nil="true"/>
		            </NamespaceAvailability>
	            </content>
            </entry>*/

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace netservices = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";
            XDocument document;

            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                document = XDocument.Parse(reader.ReadToEnd());
            }
            var root = document.Element(atom + "entry");
            var content = root.Element(atom + "content");
            var @namespace = content.Element(netservices + "NamespaceAvailability");
            var result = @namespace.Element(netservices + "Result").Value;
            IsAvailable = !bool.Parse(result);
            SitAndWait.Set();
        }
    }
}