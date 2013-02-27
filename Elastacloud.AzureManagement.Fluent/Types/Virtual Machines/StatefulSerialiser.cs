/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;


namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    public abstract class StatefulSerialiser
    {
        public abstract PersistentVMRole GetVmRole();

        #region Helpers

        protected string GetStringValue(XElement element)
        {
            return element != null ? element.Value : null;
        }

        protected int GetIntValue(XElement element)
        {
            return element != null ? int.Parse(element.Value) : 0;
        }

        protected XNamespace Namespace { get { return Namespaces.NsWindowsAzure; } }
        protected XNamespace TypeNamespace
        {
            get { return XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance"); }
        }

        protected T GetEnumValue<T>(XElement element)
        {
            if (element != null)
                return (T)Enum.Parse(typeof(T), element.Value);
            return default(T);
        }

        #endregion
    }
}
