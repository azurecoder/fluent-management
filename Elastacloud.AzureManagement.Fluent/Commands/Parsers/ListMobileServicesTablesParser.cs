/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// parses the response from a mobile services table list call
    /// </summary>
    internal class ListMobileServicesTableParser : BaseParser
    {
        /// <summary>
        /// Used to construct a list of mobile services tables from an xml response
        /// </summary>
        public ListMobileServicesTableParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<MobileServiceTable>();
        }

        /// <summary>
        /// Pulls back all of the details about the mobile services table
        /// </summary>
        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + RootElement).Elements(GetSchema() + "Table");
            foreach (var service in rootElements.Select(mobileService => new MobileServiceTable()
            {
                TableName = mobileService.Element(GetSchema() + "Name").Value,
                SizeInBytes = int.Parse(mobileService.Element(GetSchema() + "Metrics").Element(GetSchema() + "SizeBytes").Value),
                NumberOfIndexes = int.Parse(mobileService.Element(GetSchema() + "Metrics").Element(GetSchema() + "IndexCount").Value),
                NumberOfRecords = int.Parse(mobileService.Element(GetSchema() + "Metrics").Element(GetSchema() + "RecordCount").Value)
            }))
            {
                CommandResponse.Add(service);
            }
        }

        #region Overrides of BaseParser

        /// <summary>
        /// Returns the root table element
        /// </summary>
        internal override string RootElement
        {
            get { return ListMobileServiceTablesParser; }
        }
        /// <summary>
        /// Returns the mobile services namespace
        /// </summary>
        protected override XNamespace GetSchema()
        {
            return Namespaces.NsWindowsAzureMobileServices;
        }

        #endregion
    }
}