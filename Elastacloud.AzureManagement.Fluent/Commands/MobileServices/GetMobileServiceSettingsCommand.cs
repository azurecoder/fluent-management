/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.IO;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class GetMobileServiceSettingsCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal GetMobileServiceSettingsCommand(string serviceName, string path, string overrideVerb = HttpVerbGet)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = serviceName;
            HttpVerb = overrideVerb;
            Accept = "application/json"; 
            HttpCommand = string.Format("mobileservices/{0}/{1}", serviceName.ToLower(), path);
        }

        public string JsonResult { get; set; }
        /// <summary>
        /// populates the JSON string to parse on the return trip
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            using(var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                JsonResult = reader.ReadToEnd();
            }
            SitAndWait.Set();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetMobileServiceSettingsCommand";
        }
    }
}