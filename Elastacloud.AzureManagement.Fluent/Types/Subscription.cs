/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// A class which is used to provide subscription information via the the .publishsettings file
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// The friendly name of the subscription
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// the GUID subscription id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The managementcertificate used by the subscription
        /// </summary>
        public X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// The mangagement url - usually the service management api Url
        /// </summary>
        public string ManagementUrl { get; set; }
    }
}
