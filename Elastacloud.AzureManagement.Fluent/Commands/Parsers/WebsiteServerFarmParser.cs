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
using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Used to parse the Xml response back from the <Sites></Sites>
    /// </summary>
    internal class WebsiteServerFarmParser : BaseParser
    {
        /// <summary>
        /// Used to construct a website parser
        /// </summary>
        /// <param name="document"></param>
        public WebsiteServerFarmParser(XDocument document)
            : base(document)
        {
            CommandResponse = ServerFarm = new ServerFarm();
        }

        /// <summary>
        /// Contains a collections of websites
        /// </summary>
        public ServerFarm ServerFarm { get; private set; }
        /// <summary>
        /// The namespace that contains details in an array string value
        /// </summary>
        public const string SerialisationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return WebsiteServerFarm; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        internal override void Parse()
        {
            // <ServerFarm xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><CurrentNumberOfWorkers>2</CurrentNumberOfWorkers><CurrentWorkerSize>Small</CurrentWorkerSize><Name>DefaultServerFarm</Name><NumberOfWorkers>2</NumberOfWorkers><Status>Ready</Status><WorkerSize>Small</WorkerSize></ServerFarm>
            XElement rootElement = Document.Element(GetSchema() + RootElement);
            // get the compute modefrom the instance 
            if (rootElement.Element(GetSchema() + "Name") != null)
            {
                ServerFarm.Name = rootElement.Element(GetSchema() + "Name").Value;
            }
            // get the name of the site as in xxxx.azurewebsites.net
            if (rootElement.Element(GetSchema() + "NumberOfWorkers") != null)
            {
                ServerFarm.InstanceCount = int.Parse(rootElement.Element(GetSchema() + "NumberOfWorkers").Value);
            }
            // test to see whether the sitehas been enabled
            if (rootElement.Element(GetSchema() + "WorkerSize") != null)
            {
                ServerFarm.InstanceSize =
                    (InstanceSize)
                    Enum.Parse(typeof (InstanceSize), rootElement.Element(GetSchema() + "WorkerSize").Value, true);
            }

            CommandResponse = ServerFarm;
        }

        #endregion
    }
}