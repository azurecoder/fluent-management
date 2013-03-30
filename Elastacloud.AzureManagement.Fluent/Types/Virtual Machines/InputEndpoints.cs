/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Maintains the collection of InputEndpoint values 
    /// </summary>
    public class InputEndpoints : ICustomXmlSerializer
    {
        /// <summary>
        /// The local collection of InputEndpoint values
        /// </summary>
        private readonly List<InputEndpoint> _endpoints;

        /// <summary>
        /// Creates a new instance of the InputEndpoints collection class
        /// </summary>
        public InputEndpoints()
        {
            _endpoints = new List<InputEndpoint>();
        }

        /// <summary>
        /// Adds an endpoint checking to see whether the name and ports have been taken 
        /// </summary>
        /// <param name="endpoint">An InputEndpoint which should be added to the collection</param>
        public void AddEndpoint(InputEndpoint endpoint)
        {
            // check to see whether the endpoint exists or not
            IEnumerable<InputEndpoint> endpoints =
                _endpoints.Where(a => a.LocalPort == endpoint.LocalPort || (a.Port == endpoint.LocalPort && a.Port != 0)
                                      || a.EndpointName == endpoint.EndpointName);
            if (endpoints.Any())
                throw new ApplicationException(
                    "An endpoint containing the Local/Remote port and/or endpoint name already exists");
            // continue if it doesn't
            _endpoints.Add(endpoint);
        }

        public InputEndpoint this[int index]
        {
            get
            {
                if(index < 0 || index >= _endpoints.Count)
                    throw new ArgumentOutOfRangeException("endpoint index out of range");
                return _endpoints[index];
            }
        }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var inputEndpoints = new XElement(Namespaces.NsWindowsAzure + "InputEndpoints");
            // iterate through all of the various input endpoints and 
            foreach (InputEndpoint inputEndpoint in _endpoints)
            {
                inputEndpoints.Add(inputEndpoint.GetXmlTree());
            }
            return inputEndpoints;
        }

        #endregion
    }
}