using System;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.ServiceBus
{
    /// <summary>
    /// Used to get the oAuth WRAP token from ACS for the service bus instance 
    /// </summary>
    internal class GetManagementTokenCommand : ServiceCommand
    {
        /// <summary>
        /// The format of the service bus uri request
        /// </summary>
        public const string RequestUriFormat = "https://{0}-sb.accesscontrol.windows.net";

        /// <summary>
        /// Used to setup the parameters - n.b. the service bus namespace for acs becomes @namespace-sb.accesscontrol.windows.net
        /// </summary>
        internal GetManagementTokenCommand(string @namespace, string managementKey)
        {
            //TODO: Make an intermediate ServiceBusCommand class
            Namespace = @namespace;
            ManagementKey = managementKey;
            BaseRequestUri = String.Format(RequestUriFormat, @namespace);
            HttpVerb = HttpVerbPost;
            ServiceType = "v2";
            OperationId = "OAuth2-13";
            UseCertificate = false;
            ContentType = "application/x-www-form-urlencoded";
        }

        /// <summary>
        /// The root namespace of the service bus 
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// The Management Key used to access the Access Control Service
        /// </summary>
        public string ManagementKey { get; set; }

        /// <summary>
        /// Creates a post body which contains meta information to access the management service with the appropriate credentials
        /// </summary>
        protected override string CreatePayload()
        {
            string postBody = "grant_type=client_credentials";
            postBody += "&client_id=SBManagementClient";
            postBody += "&client_secret=" + UrlHelper.Encode(ManagementKey);
            postBody += "&scope=" + UrlHelper.Encode(BaseRequestUri) + "/v2/mgmt/service/";

            return postBody;
        }
    }
}