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
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Websites
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class UpdateWebsiteConfigCommand : ServiceCommand
    {
        public const string WebsitePostfix = ".azurewebsites.net";
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal UpdateWebsiteConfigCommand(Website website)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            Website = website;
            HttpVerb = HttpVerbPut;
            AdditionalHeaders["Accept-Encoding"] = "gzip, deflate";
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/sites/{1}/config", Website.Webspace, Website.Name);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }

        protected Website Website { get; set; }

       
        protected override string CreatePayload()
        {
            XNamespace xmlns = "http://schemas.microsoft.com/windowsazure";
            XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace a = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

            XName iNamespace = XNamespace.Xmlns + "i";
            XName iNil = i + "nil";
            XName aArray = XNamespace.Xmlns + "a";
            XName aString = a + "string";

            if (Website.Config == null)
            {
                throw new FluentManagementException("Website configuration is not currently available", "UpdateWebsiteConfigCommand");
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", ""));
            var root = new XElement(xmlns + "SiteConfig", new XAttribute(iNamespace, i), new XAttribute(aArray, a));

            #region boolean properties

            // get the compute modefrom the instance 
            var detailedErrorLoggingEnabled = new XElement(xmlns + "DetailedErrorLoggingEnabled", Website.Config.DetailedErrorLoggingEnabled);
            // get the compute modefrom the instance 
            var httpLoggingEnabled = new XElement(xmlns + "HttpLoggingEnabled", Website.Config.HttpLoggingEnabled);
            // get the compute modefrom the instance 
            var requestTracingEnabled = new XElement(xmlns + "RequestTracingEnabled", Website.Config.RequestTracingEnabled);
            // get the compute modefrom the instance 
            var use32BitWorkerProcess = new XElement(xmlns + "Use32BitWorkerProcess", Website.Config.Use32BitWorkerProcess);

            #endregion

            #region string and int values

            XElement netFrameworkVersion = null, phpVersion = null, publishingPassword = null, publishingUsername = null, numberOfWorkers = null;
            // this is the type of source control used
            var scmType = new XElement(xmlns + "ScmType", Website.Config.ScmType);
            // add the publishing password
            if (Website.Config.PhpVersion != null)
            {
                netFrameworkVersion = new XElement(xmlns + "NetFrameworkVersion", Website.Config.NetFrameworkVersion);
            }
            // add the publishing password
            if (Website.Config.PhpVersion != null)
            {
                phpVersion = new XElement(xmlns + "PhpVersion", Website.Config.PhpVersion);
            }
            // add the publishing password
            if (Website.Config.PublishingPassword != null)
            {
                publishingPassword = new XElement(xmlns + "PublishingPassword", Website.Config.PublishingPassword);
            }
            // add the publishing username
            if (Website.Config.PublishingUsername != null)
            {
                publishingUsername = new XElement(xmlns + "PublishingUsername", Website.Config.PublishingUsername);
            }
            // add the number of workers 
            if (Website.Config.NumberOfWorkers > 0)
            {
                numberOfWorkers = new XElement(xmlns + "NumberOfWorkers", Website.Config.NumberOfWorkers);
            }

            #endregion

            #region collection values

            // build appsettings
            var appSettings = new XElement(xmlns + "AppSettings");
            foreach (var appSetting in Website.Config.AppSettings)
            {
                appSettings.Add(new XElement(xmlns + "NameValuePair", new XElement(xmlns + "Name", appSetting.Key), new XElement(xmlns + "Value", appSetting.Value)));                
            }
            // build metadata
            var metadatas = new XElement(xmlns + "Metadata");
            foreach (var metadata in Website.Config.Metadata)
            {
                metadatas.Add(new XElement(xmlns + "NameValuePair", new XElement(xmlns + "Name", metadata.Key), new XElement(xmlns + "Value", metadata.Value)));
            }
            // build connection strings
            var connectionStrings = new XElement(xmlns + "ConnectionStrings");
            foreach (var connectionString in Website.Config.ConnectionStrings)
            {
                connectionStrings.Add(new XElement(xmlns + "ConnStringInfo",
                                                   new XElement(xmlns + "ConnectionString", connectionString.ConnectionString),
                                                   new XElement(xmlns + "Name", connectionString.Name),
                                                   new XElement(xmlns + "Type", connectionString.Type)));
            }
            // add the default documents
            var defaultDocuments = new XElement(xmlns + "DefaultDocuments");
            foreach (var defaultDocument in Website.Config.DefaultDocuments)
            {
                defaultDocuments.Add(new XElement(a + "string", defaultDocument));
            }
                
            #endregion

            // add the elements to the root node
            root.Add(appSettings);
            root.Add(connectionStrings);
            root.Add(defaultDocuments);
            root.Add(detailedErrorLoggingEnabled);
            root.Add(httpLoggingEnabled);
            root.Add(metadatas);
            if (netFrameworkVersion != null) root.Add(netFrameworkVersion);
            if (numberOfWorkers != null) root.Add(numberOfWorkers);
            if (phpVersion != null) root.Add(phpVersion);
            if (publishingPassword != null) root.Add(publishingPassword);
            if (publishingUsername != null) root.Add(publishingUsername);
            root.Add(requestTracingEnabled);
            root.Add(scmType);
            root.Add(use32BitWorkerProcess);
            doc.Add(root);
                            
            return doc.ToStringFullXmlDeclaration();
        }
    }
}