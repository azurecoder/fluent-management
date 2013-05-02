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
using System.Net;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Websites
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class CreateWebsiteCommand : ServiceCommand
    {
        public const string WebsitePostfix = ".azurewebsites.net";
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal CreateWebsiteCommand(Website website)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            Website = website;
            HttpVerb = HttpVerbPost;
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/sites/", Website.Webspace);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }

        protected Website Website { get; set; }

        /*<Site xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
            <AdminEnabled i:nil="true"/>
            <AvailabilityState>Normal</AvailabilityState>
            <Enabled i:nil="true"/>
            <EnabledHostNames i:nil="true" xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays"/>
            <HostNameSslStates i:nil="true"/>
                <HostNames xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
                    <a:string>clitest37.azurewebsites.net</a:string>
                </HostNames>
                <Name>clitest37</Name>
                <Owner i:nil="true"/>
                <RepositorySiteName i:nil="true"/>
                <SSLCertificates i:nil="true"/>
                <SelfLink i:nil="true"/>
                <SiteMode i:nil="true"/>
                <SiteProperties i:nil="true"/>
                <State i:nil="true"/>
                <UsageState>Normal</UsageState>
                <WebSpace>northeuropewebspace</WebSpace>
                <WebSpaceToCreate>
                    <AvailabilityState>Normal</AvailabilityState>
                    <ComputeMode>Dedicated</ComputeMode>
                    <CurrentNumberOfWorkers>1</CurrentNumberOfWorkers>
                    <CurrentWorkerSize>Small</CurrentWorkerSize>
                    <Name>northeuropewebspace</Name>
                    <NumberOfWorkers>1</NumberOfWorkers>
                    <Status>Ready</Status>
                    <WorkerSize>Small</WorkerSize>
                </WebSpaceToCreate>
            </Site>*/
        protected override string CreatePayload()
        {
            XNamespace xmlns = "http://schemas.microsoft.com/windowsazure";
            XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace a = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

            XName iNamespace = XNamespace.Xmlns + "i";
            XName iNil = i + "nil";
            XName aArray = XNamespace.Xmlns + "a";
            XName aString = a + "string";

            var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", ""),
                    new XElement(xmlns + "Site", new XAttribute(iNamespace, i),
                            new XElement(xmlns + "AdminEnabled", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "AvailabilityState", Website.WebsiteParameters.AvailabilityState),
                            new XElement(xmlns + "Enabled", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "EnabledHostNames", new XAttribute(iNil, "true"), new XAttribute(aArray, a)),
                            new XElement(xmlns + "HostNameSslState", new XAttribute(iNil, "true"), new XAttribute(aArray, a)),
                            new XElement(xmlns + "HostNames", new XAttribute(aArray, a),
                                new XElement(aString, Website.Name + WebsitePostfix)),
                                //new XElement(aString, Website.Name + ".scm" + WebsitePostfix)),
                            new XElement(xmlns + "Name", Website.Name),
                            new XElement(xmlns + "Owner", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "RepositorySiteName", Website.Name),
                            new XElement(xmlns + "SSLCertificates", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "SelfLink", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "SiteMode", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "SiteProperties", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "State", new XAttribute(iNil, "true")),
                            new XElement(xmlns + "UsageState", Website.Usage),
                            new XElement(xmlns + "WebSpace", Website.Webspace),
                            new XElement(xmlns + "WebSpaceToCreate",
                                new XElement(xmlns + "AvailabilityState", Website.WebsiteParameters.AvailabilityState),
                                new XElement(xmlns + "ComputeMode", Website.ComputeMode),
                                new XElement(xmlns + "CurrentNumberOfWorkers", Website.WebsiteParameters.CurrentNumberOfWorkers),
                                new XElement(xmlns + "CurrentWorkerSize", Website.WebsiteParameters.CurrentWorkerSize),
                                new XElement(xmlns + "Name", Website.Webspace),
                                new XElement(xmlns + "NumberOfWorkers", Website.WebsiteParameters.CurrentNumberOfWorkers),
                                new XElement(xmlns + "Status", Website.State),
                                new XElement(xmlns + "WorkerSize", Website.WebsiteParameters.CurrentWorkerSize))));


            return doc.ToStringFullXmlDeclaration();
        }
    }
}