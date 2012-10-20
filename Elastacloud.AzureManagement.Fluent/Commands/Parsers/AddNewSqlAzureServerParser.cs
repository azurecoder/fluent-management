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

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Parses the response from the SQL Azure Add New command
    /// </summary>
    internal class AddNewSqlAzureServerParser : BaseParser
    {
        /// <summary>
        /// Constructs the parser from the Xml document
        /// </summary>
        /// <param name="response">An Xml document</param>
        public AddNewSqlAzureServerParser(XDocument response) : base(response)
        {
            CommandResponse = String.Empty;
        }

        /// <summary>
        /// Parses the response element from a command response 
        /// </summary>
        internal override void Parse()
        {
            CommandResponse = (string) Document.Element(GetSchema() + RootElement);
        }

        #region Overrides of BaseParser

        /// <summary>
        /// The root element of the response from the Add New Sql Azure server
        /// </summary>
        internal override string RootElement
        {
            get { return AddNewSqlAzureServerParser; }
        }

        /// <summary>
        /// Gets the schema definition for new Sql Azure
        /// </summary>
        /// <returns>Gets the schema definition</returns>
        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(SqlAzureSchema);
        }

        #endregion
    }
}