/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Contains a list of persistent VM roles which will be executed in the deployment
    /// </summary>
    public class RoleList : ICustomXmlSerializer
    {
        /// <summary>
        /// Creates a rolelist with an empty list of persistentvmrole images
        /// </summary>
        public RoleList()
        {
            Roles = new List<PersistentVMRole>();
        }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "RoleList");
            Roles.ForEach(a => element.Add(a.GetXmlTree()));
            return element;
        }

        #endregion

        /// <summary>
        /// Lists the persistent VM Roles which are bound to the deployment in this list
        /// </summary>
        public List<PersistentVMRole> Roles { get; set; }
    }
}