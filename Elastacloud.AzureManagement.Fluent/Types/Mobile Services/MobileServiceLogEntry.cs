/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Elastacloud.AzureManagement.Fluent.Types.MobileServices
{
    /// <summary>
    /// Defines a mobile services table
    /// </summary>
    public class MobileServiceLogEntry
    {
        /// <summary>
        /// The type of level logged
        /// </summary>
        [JsonProperty(PropertyName = "type"), JsonConverter(typeof(StringEnumConverter))]
        public LogLevelType Type { get; set; }
        /// <summary>
        /// The time that the log entry was created
        /// </summary>
        [JsonProperty(PropertyName = "timeCreated")]
        public DateTime TimeCreated { get; set; }
        /// <summary>
        /// The event log source
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
        /// <summary>
        /// The message that has been inserted into the event log
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}