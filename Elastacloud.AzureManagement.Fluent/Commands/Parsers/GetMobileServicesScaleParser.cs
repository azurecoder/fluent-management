﻿/************************************************************************************************************
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
using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Used to parse the Xml response back from the <Sites></Sites>
    /// </summary>
    internal class GetMobileServicesScaleParser : BaseParser
    {
        /// <summary>
        /// Used to construct a website parser
        /// </summary>
        /// <param name="document"></param>
        public GetMobileServicesScaleParser(XDocument document)
            : base(document)
        {
            CommandResponse = new ScaleSettings();
        }

        /// <summary>
        /// Contains a collections of websites
        /// </summary>
        public ScaleSettings Properties { get; private set; }
        /// <summary>
        /// The namespace that contains details in an array string value
        /// </summary>
        public const string SerialisationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return WebsiteParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(MobileServicesSchema);
        }

        internal override void Parse()
        {
            Properties = new ScaleSettings();

            XNamespace serialisationNamespace = SerialisationNamespace;
            XElement rootElements = Document.Element(GetSchema() + "ScaleSettings");
            // get the compute modefrom the instance 
            if (rootElements.Element(GetSchema() + "Tier") != null)
            {
                Properties.ComputeMode =
                    (ComputeMode)
                    Enum.Parse(typeof (ComputeMode), rootElements.Element(GetSchema() + "Tier").Value, true);
            }
            // test to see whether the sitehas been enabled
            if (rootElements.Element(GetSchema() + "NumberOfInstances") != null)
            {
                Properties.InstanceCount = int.Parse(rootElements.Element(GetSchema() + "NumberOfInstances").Value);
            }

            CommandResponse = Properties;
        }

        #endregion
    }
}