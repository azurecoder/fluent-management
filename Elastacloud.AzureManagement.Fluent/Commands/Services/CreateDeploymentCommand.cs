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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to a create a deployment for a web or worker role given some specific deployment parameters
    /// </summary>
    internal class CreateDeploymentCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a deployment to a particular cloud service
        /// </summary>
        /// <param name="serviceName">the name of the cloud service</param>
        /// <param name="deploymentName">The name of the deployment</param>
        /// <param name="packageUri">The blob store and endpoint address where the package has been uploaded to</param>
        /// <param name="config">The base64 encoded config of .cscfg file</param>
        /// <param name="slot">The deployment slot - can be production or staging</param>
        /// <param name="startDeployment">an optional parameter which defaults to true as to whether the deployment should be started when complete</param>
        /// <param name="treatWarningsAsErrors">an optional parameter set to false - any warnings (such as SLA or config violations) will be treated as an error and stop the deployment</param>
        internal CreateDeploymentCommand(string serviceName, string deploymentName, string packageUri, string config,
                                         DeploymentSlot slot, bool startDeployment = true,
                                         bool treatWarningsAsErrors = false)
        {
            OperationId = "hostedservices";
            ServiceType = "services";
            Name = serviceName;
            DeploymentSlot = slot;
            DeploymentName = deploymentName;
            PackageUri = packageUri;
            Config = config;
            HttpCommand = Name + "/deploymentslots/" + slot.ToString().ToLower();
            StartDeploymentAutomatically = startDeployment;
            TreatWarningsAsErrors = treatWarningsAsErrors;
        }

        /// <summary>
        /// The deployment slot being used production|staging
        /// </summary>
        internal DeploymentSlot DeploymentSlot { get; set; }

        /// <summary>
        /// The name of the deployment
        /// </summary>
        internal string DeploymentName { get; set; }

        /// <summary>
        /// The blob storage package uri of the deployment
        /// </summary>
        internal string PackageUri { get; set; }

        /// <summary>
        /// The base64 config of the deployment
        /// </summary>
        internal string Config { get; set; }

        /// <summary>
        /// Whether the deployment should be started automatically
        /// </summary>
        internal bool StartDeploymentAutomatically { get; set; }

        /// <summary>
        /// Whether errors should be treated as warnings
        /// </summary>
        internal bool TreatWarningsAsErrors { get; set; }

        //https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot-name>
        /// <summary>
        /// The Xml payload that is created and sent to the Fabric with the create deployment parameters
        /// </summary>
        /// <returns>A string Xml document representation</returns>
        protected override string CreateXmlPayload()
        {
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "CreateDeployment",
                             new XElement(ns + "Name", Name),
                             new XElement(ns + "PackageUrl", PackageUri),
                             new XElement(ns + "Label", Convert.ToBase64String(Encoding.UTF8.GetBytes(DeploymentName))),
                             new XElement(ns + "Configuration", Config),
                             new XElement(ns + "StartDeployment", StartDeploymentAutomatically.ToString().ToLower()),
                             new XElement(ns + "TreatWarningsAsError", TreatWarningsAsErrors.ToString().ToLower())));
            return doc.ToStringFullXmlDeclaration();
        }
    }
}