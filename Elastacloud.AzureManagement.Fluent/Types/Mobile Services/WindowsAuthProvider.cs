/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using Newtonsoft.Json;

namespace Elastacloud.AzureManagement.Fluent.Types.MobileServices
{
    /// <summary>
    /// This class is the mobile services provider and is used to pull and push details of the provider
    /// </summary>
    public class WindowsAuthProvider
    {
        /// <summary>
        /// Used to construct a mobile services provider
        /// </summary>
        public WindowsAuthProvider(string packageSid, string clientId, string clientSecret)
        {
            PackageSid = packageSid;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// The mobile services provider
        /// </summary>
        [JsonProperty(PropertyName = "packageSid")]
        public string PackageSid { get; set; }
        /// <summary>
        /// The App Id from the oAuth 
        /// </summary>
        [JsonProperty(PropertyName = "clientId")]
        public string ClientId { get; set; }
        /// <summary>
        /// The secret from the oauth
        /// </summary>
        [JsonProperty(PropertyName = "clientSecret")]
        public string ClientSecret { get; set; }
    }
}
