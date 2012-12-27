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
    public class MobileServicesAuthProvider
    {
        /// <summary>
        /// Used to construct a mobile services provider
        /// </summary>
        public MobileServicesAuthProvider(string provider, string appId, string secret)
        {
            Provider = provider;
            AppId = appId;
            Secret = secret;
        }

        /// <summary>
        /// The mobile services provider
        /// </summary>
        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }
        /// <summary>
        /// The App Id from the oAuth 
        /// </summary>
        [JsonProperty(PropertyName = "appId")]
        public string AppId { get; set; }
        /// <summary>
        /// The secret from the oauth
        /// </summary>
        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }
    }
}
