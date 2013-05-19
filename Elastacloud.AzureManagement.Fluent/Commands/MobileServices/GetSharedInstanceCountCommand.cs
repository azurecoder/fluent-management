/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    // https://management.core.windows.net/sdgfhjsdf/services/mobileservices/webspaces/EASTUSWEBSPACE
    internal class GetSharedInstanceCountCommand : ServiceCommand
    {
        internal GetSharedInstanceCountCommand(string webSpace)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            HttpVerb = HttpVerbGet;
            HttpCommand = "webspaces/" + webSpace.ToUpper();
        }

        public WebspaceProperties WebspaceProperties { get; set; }
        // TODO: parse the response on the way back 
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            WebspaceProperties = Parse(webResponse, BaseParser.WebsiteListParser, new WebspacePropertiesParser(null));

            SitAndWait.Set();
        }

        public override string ToString()
        {
            return "GetSharedInstanceCountCommand";
        }
    }

   
}
