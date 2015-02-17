/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    /// <summary>
    /// Defines an interface which can perform operations on the service bus
    /// </summary>
    public interface IServiceBusClient
    {
        /// <summary>
        /// Creates a namespace given a name value 
        /// </summary>
        /// <param name="name">The name of the namespace that has to follow the prescribed rules</param>
        /// <param name="location">The location of the namespace default to N. Europe</param>
        void CreateNamespace(string name, string location = LocationConstants.NorthEurope);
        /// <summary>
        /// Checks to see whether a given namespace exists
        /// </summary>
        bool CheckNamespaceExists(string name);
        /// <summary>
        /// Deletes a namespace and throws an exception if the namespace doesn't exist
        /// </summary>
        void DeleteNamespace(string name);
        /// <summary>
        /// Returns a list of service bus namespaces
        /// </summary>
        IEnumerable<string> GetServiceBusNamspaceList(string location);
        /// <summary>
        /// Gets a service bus connection string given a namespace name
        /// </summary>
        string GetServiceBusConnectionString(string @namespace, string ruleName);
        /// <summary>
        /// The name of the service bus namespace
        /// </summary>
        string Namespace { get; set; }
    }
}