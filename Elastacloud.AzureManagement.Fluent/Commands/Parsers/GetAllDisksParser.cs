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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Parses the response of a list of disks within a subscription
    /// </summary>
    internal class GetAllDisksParser : BaseParser
    {
        public GetAllDisksParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<VirtualDisk>();
        }

        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + "Disks")
                .Elements(GetSchema() + "Disk");
            foreach (XElement disk in rootElements)
            {
                var virtualDisk = new VirtualDisk();
                // get the affinity group if it exists
                if (disk.Element(GetSchema() + "AffinityGroup") != null)
                {
                    virtualDisk.AffinityGroup = disk.Element(GetSchema() + "AffinityGroup").Value;
                }
                // get the Location if it exists
                if (disk.Element(GetSchema() + "Location") != null)
                {
                    virtualDisk.Location = disk.Element(GetSchema() + "Location").Value;
                }
                // get the OS if it exists
                if (disk.Element(GetSchema() + "OS") != null)
                {
                    virtualDisk.OS = disk.Element(GetSchema() + "OS").Value;
                }
                // get the medialink if it exists
                if (disk.Element(GetSchema() + "MediaLink") != null)
                {
                    virtualDisk.MediaLink = disk.Element(GetSchema() + "MediaLink").Value;
                }
                // get the name if it exists
                if (disk.Element(GetSchema() + "Name") != null)
                {
                    virtualDisk.Name = disk.Element(GetSchema() + "Name").Value;
                }
                // get the sourceimagename if it exists
                if (disk.Element(GetSchema() + "SourceImageName") != null)
                {
                    virtualDisk.SourceImageName = disk.Element(GetSchema() + "SourceImageName").Value;
                }
                // get the sourceimagename if it exists
                if (disk.Element(GetSchema() + "LogicalSizeInGB") != null)
                {
                    virtualDisk.LogicalSizeInGB = int.Parse(disk.Element(GetSchema() + "LogicalSizeInGB").Value);
                }
                // check if this is attached to a vm
                if (disk.Element(GetSchema() + "AttachedTo") != null)
                {
                    virtualDisk.VM = new AttachedVM()
                        {
                            CloudServiceName =
                                disk.Element(GetSchema() + "AttachedTo").Element(GetSchema() + "HostedServiceName").Value,
                            DeploymentName = disk.Element(GetSchema() + "AttachedTo").Element(GetSchema() + "DeploymentName").Value,
                            RoleName = disk.Element(GetSchema() + "AttachedTo").Element(GetSchema() + "RoleName").Value
                        };
                }

                CommandResponse.Add(virtualDisk);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return GetVirtualDisksParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}