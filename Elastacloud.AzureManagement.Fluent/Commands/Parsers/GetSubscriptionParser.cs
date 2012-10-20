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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    internal class GetSubscriptionParser : BaseParser
    {
        public GetSubscriptionParser(XDocument document) : base(document)
        {
            CommandResponse = new SubscriptionInformation();
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return GetSubscriptionParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        internal override void Parse()
        {
            XElement rootElements = Document.Element(GetSchema() + GetSubscriptionParser);
            var subscriptionItem = new SubscriptionInformation
                                       {
                                           AccountAdminLiveId =
                                               (string) rootElements.Element(GetSchema() + "AccountAdminLiveEmailId"),
                                           SubscriptionId =
                                               (string) rootElements.Element(GetSchema() + "SubscriptionId"),
                                           SubscriptionName =
                                               (string) rootElements.Element(GetSchema() + "SubscriptionName"),
                                           ServiceAdminLiveId =
                                               (string) rootElements.Element(GetSchema() + "ServiceAdminLiveEmailId"),
                                           CurrentCoreCount =
                                               (int) rootElements.Element(GetSchema() + "CurrentCoreCount"),
                                           CurrentHostedServices =
                                               (int) rootElements.Element(GetSchema() + "CurrentHostedServices"),
                                           CurrentStorageAccounts =
                                               (int) rootElements.Element(GetSchema() + "CurrentStorageAccounts"),
                                           MaxCoreCount = (int) rootElements.Element(GetSchema() + "MaxCoreCount"),
                                           MaxHostedServices =
                                               (int) rootElements.Element(GetSchema() + "MaxHostedServices"),
                                           MaxStorageAccounts =
                                               (int) rootElements.Element(GetSchema() + "MaxStorageAccounts"),
                                           SubscriptionStatus =
                                               (SubscriptionStatus)
                                               Enum.Parse(typeof (SubscriptionStatus),
                                                          rootElements.Element(GetSchema() + "SubscriptionStatus").Value)
                                       };
            CommandResponse = subscriptionItem;
        }

        #endregion
    }
}