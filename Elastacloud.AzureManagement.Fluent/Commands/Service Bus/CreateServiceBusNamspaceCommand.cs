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
	internal class CreateServiceBusNamespaceCommand : ServiceCommand
	{
        internal CreateServiceBusNamespaceCommand(string name, string location = LocationConstants.NorthEurope)
		{
            Name = name;
			Location = location;
			HttpVerb = HttpVerbPut;
			ServiceType = "services";
			OperationId = "ServiceBus";
            HttpCommand = "Namespaces/" + name;
            ContentType = "application/atom+xml";
		}

		protected override string CreatePayload()
		{
			/*<entry xmlns='http://www.w3.org/2005/Atom'>  
             * <content type='application/xml'>
             * <NamespaceDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\"> 
             * <Region>{0}</Region>
             * </NamespaceDescription>  
             * </content>
             * </entry>*/
            // Serialize NamespaceDescription, if additional properties needs to be specified http://msdn.microsoft.com/en-us/library/jj873988.aspx

            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace iXmlSchema = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace serviceBusSchema = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";

		    XName i = iXmlSchema + "i";
		    var doc = new XDocument(
		        new XElement(ns + "entry",
		                     new XElement(ns + "content", new XAttribute("type", "application/xml"),
		                                  new XElement(serviceBusSchema + "NamespaceDescription", new XAttribute(i, "http://www.w3.org/2001/XMLSchema-instance"),
                                                       new XElement(serviceBusSchema + "Region", Location)))));
			return doc.ToStringFullXmlDeclaration();
		}

        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            base.ResponseCallback(webResponse);
        }
	}
}