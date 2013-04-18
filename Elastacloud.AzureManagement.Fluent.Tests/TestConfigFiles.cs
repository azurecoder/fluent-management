using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Tests.Properties;
using Elastacloud.AzureManagement.Fluent.Types;
using NUnit.Framework;
using FluentAssertions;

namespace Elastacloud.AzureManagement.Fluent.Tests
{
    [TestFixture]
    public class TestConfigFiles
    {
        private const string NodeMappingValue = "nodemappingelastakca78kt2elastakca78kt2bc5cafb3e13545ca144b133f";
        private const string NodeMappingName = "Microsoft.Hpc.Azure.NodeMapping";
        private static XDocument _document;

        [TestFixtureSetUp]
        public void StartUp()
        {
            byte[] cloudFile = Resources.ElastaConfiguration_Cloud;
            var stream = new MemoryStream(cloudFile);
            {
                _document = XDocument.Load(stream);
            }
        }

        [Test]
        public void TestGetRoleListForThreeRoles()
        {
            var cloudConfig = CscfgFile.GetAdHocInstance(_document);
            List<string> roleNames = cloudConfig.GetRoleNameList();
            roleNames.Count.Should().Be(3);
        }

        [Test]
        public void TestGetNodeMappingSettingForHeadnodeRole()
        {
            const string roleName = "Headnode";
            var cloudConfig = CscfgFile.GetAdHocInstance(_document);
            string value = cloudConfig.GetSettingForRole(NodeMappingName, roleName);
            value.Should().Be(NodeMappingValue);
        }

        [Test]
        public void TestGetRoleInstanceCountForHeadnodeRoleShouldEqualOne()
        {
            const string roleName = "Headnode";
            var cloudConfig = CscfgFile.GetAdHocInstance(_document);
            int count = cloudConfig.GetInstanceCountForRole(roleName);
            count.Should().Be(1);
        }

        [Test]
        public void TestGetSettingCheckInAllRoles()
        {
            var cloudConfig = CscfgFile.GetAdHocInstance(_document);
            cloudConfig.DoesSettingExist(NodeMappingName).Should().BeTrue("The setting should exist in at least one of the roles in the file");
        }
    }
}
