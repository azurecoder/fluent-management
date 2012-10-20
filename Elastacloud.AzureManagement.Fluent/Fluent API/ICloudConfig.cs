/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent
{
    /// <summary>
    /// Interface used to define the semantics of cloud configuration
    /// </summary>
    public interface ICloudConfig
    {
        /// <summary>
        /// the rolename which the config is being applied to
        /// </summary>
        string Rolename { get; set; }

        /// <summary>
        /// Changes the .cscfg file based on the Xml document that is applied
        /// </summary>
        /// <param name="document">The input document</param>
        /// <returns>A new and transient xml document of the config</returns>
        XDocument ChangeConfig(XDocument document);

        /// <summary>
        /// Used to change the .csdef config file
        /// </summary>
        /// <param name="document">the input document</param>
        /// <returns>A new and transient xml document representing the .csdef</returns>
        XDocument ChangeDefinition(XDocument document);
    }
}