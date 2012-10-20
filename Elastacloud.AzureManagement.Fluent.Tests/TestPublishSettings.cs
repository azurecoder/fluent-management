using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Helpers;
using NUnit.Framework;
using FluentAssertions;

namespace Elastacloud.AzureManagement.Fluent.Tests
{
    [TestFixture]
    public class TestPublishSettings
    {
        private const string Publishsettings = @"<?xml version=""1.0"" encoding=""utf-8""?><PublishData><PublishProfile PublishMethod=""AzureServiceManagementAPI"" Url=""https://management.core.windows.net/"" ManagementCertificate=""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""><Subscription Id=""aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa"" Name=""Windows Azure MSDN - Visual Studio Ultimate"" /><Subscription Id=""aaaaaaaa-8382-4990-b612-aaaaaaaaaaaa"" Name=""Windows Azure MSDN - Visual Studio Ultimate"" /><Subscription Id=""aaaaaaaa-7aac-492c-8b08-aaaaaaaaaaaa"" Name=""Test 1"" /><Subscription Id=""aaaaaaaa-e135-45ca-bc31-aaaaaaaaaaaa"" Name=""Test 2"" /><Subscription Id=""aaaaaaaa-f3f5-42d0-ba03-aaaaaaaaaaaa"" Name=""Test 3"" /></PublishProfile></PublishData>";
        private const string Filename = "elastacloud.publishsettings";

        private string _fullFileName = "";

        [TestFixtureSetUp]
        public void SetUp()
        {
            var path = Path.GetTempPath();
            _fullFileName = Path.Combine(path, Filename);
            using(var writer = File.CreateText(_fullFileName))
            {
                writer.WriteLine(Publishsettings);
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            if(File.Exists(_fullFileName))
                File.Delete(_fullFileName);
        }

        [Test]
        public void TestAddCertificateToStoreFromPublishSettings()
        {
            var cert = PublishSettingsExtractor.AddPublishSettingsToPersonalMachineStore(Publishsettings);
            cert.Should().NotBeNull("Contains a ASN1/DER encoded certificate");
            cert.HasPrivateKey.Should().BeTrue("Contains a PKCS#12 structure");
            var cert2 = PublishSettingsExtractor.FromStore(cert.Thumbprint);
            cert2.Thumbprint.Should().Be(cert.Thumbprint, "The same certificate");
            cert2.HasPrivateKey.Should().BeTrue("Same certificate as above should exist and be imported with pvk into the personal store");
            PublishSettingsExtractor.RemoveFromStore(cert2.Thumbprint);
        }

        [Test]
        public void TestGetSubscriptionsInPublishsettings()
        {
            var settings = new PublishSettingsExtractor(_fullFileName);
            var dictionary = settings.GetSubscriptions();
            dictionary.Count.Should().Be(5, "Number of subscriptions in Xml .publishsettings string");
        }

        [Test]
        public void TestGetCertificateThumbprint()
        {
            var cert = PublishSettingsExtractor.AddPublishSettingsToPersonalMachineStore(Publishsettings);

            var settings = new PublishSettingsExtractor(_fullFileName);
            string thumbprint = settings.GetCertificateThumbprint();
            thumbprint.Should().NotBeNullOrEmpty("Contains a thumbprint from the given file");
            thumbprint.Should().Be(cert.Thumbprint, "The same as the parsed certificate value");

            PublishSettingsExtractor.RemoveFromStore(cert.Thumbprint);
        }
    }
}
