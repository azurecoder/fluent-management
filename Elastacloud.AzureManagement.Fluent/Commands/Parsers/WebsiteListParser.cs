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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    internal class WebsiteListParser : BaseParser
    {
        public WebsiteListParser(XDocument document) : base(document)
        {
            CommandResponse = new SubscriptionInformation();
        }

        public List<string> Websites { get; private set; }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return WebsiteListParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        internal override void Parse()
        {
            if (Websites == null)
                Websites = new List<string>();

            XElement rootElements = Document.Element(GetSchema() + WebsiteListParser);
            foreach (var element in rootElements.Elements(GetSchema() + "WebSpace").Where(element => element.Element(GetSchema() + "Name") != null))
            {   
                Websites.Add(element.Element(GetSchema() + "Name").Value);
            }
           
            CommandResponse = Websites;
        }

        #endregion
    }
}