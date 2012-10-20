/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// Custom serialiser interface for Xml serialialisation
    /// </summary>
    public interface ICustomXmlSerializer
    {
        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        XElement GetXmlTree();
    }
}