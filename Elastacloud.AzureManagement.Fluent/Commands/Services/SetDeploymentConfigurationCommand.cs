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
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to return a list of hosted services to the client 
    /// </summary>
    internal class SetDeploymenConfigurationCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a SetDeploymenConfigurationCommand command
        /// </summary>
        // POST https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot>/?comp=config
        internal SetDeploymenConfigurationCommand(string serviceName, CscfgFile config, DeploymentSlot slot = DeploymentSlot.Production)
        {
            // need to increment the version in this request otherwise will not be able to check vm instances
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = (CloudServiceName = serviceName) + "/deploymentslots/" + (Slot = slot).ToString().ToLower() + "/?comp=config";
            Configuration = config;
            HttpVerb = HttpVerbPost;
        }

        /// <summary>
        /// The name of the hosted service
        /// </summary>
        internal string CloudServiceName { get; set; }
        /// <summary>
        /// The DeploymentSlot being used by the application
        /// </summary>
        internal DeploymentSlot Slot { get; set; }
        /// <summary>
        /// The cscfg file that is returned to be parsed
        /// </summary>
        internal CscfgFile Configuration { get; set; }

        /// <summary>
        /// Creates an Xml payload for the 
        /// </summary>
        /// <returns>The Xml string</returns>
        protected override string CreateXmlPayload()
        {
            /*<?xml version="1.0" encoding="utf-8"?>
                <ChangeConfiguration xmlns="http://schemas.microsoft.com/windowsazure">
                   <Configuration>base-64-encoded-configuration-file</Configuration>
                   <TreatWarningsAsError>true|false</TreatWarningsAsError>
                   <Mode>Auto|Manual</Mode>
                   <ExtendedProperties>
                      <ExtendedProperty>
                         <Name>property-name</Name>
                         <Value>property-value</Value>
                      </ExtendedProperty>
                   </ExtendedProperties>
                </ChangeConfiguration>*/
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "ChangeConfiguration",
                             new XElement(ns + "Configuration", Convert.ToBase64String(Encoding.UTF8.GetBytes(Configuration.ToString()))),
                             new XElement(ns + "Mode", "Auto"),
                             new XElement(ns + "TreatWarningsAsError", true.ToString().ToLower())));
            return doc.ToStringFullXmlDeclaration();

        }
    }
}