﻿using System.Globalization;
using System.IO;
using System.Threading;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using NUnit.Framework;
using FluentAssertions;

namespace Elastacloud.AzureManagement.Fluent.Tests
{
	[TestFixture]
	public class TestPublishSettings
	{
		private const string Publishsettings = @"<?xml version=""1.0"" encoding=""utf-8""?><PublishData><PublishProfile PublishMethod=""AzureServiceManagementAPI"" Url=""https://management.core.windows.net/"" ManagementCertificate=""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""><Subscription Id=""aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa"" Name=""Windows Azure MSDN - Visual Studio Ultimate"" /><Subscription Id=""aaaaaaaa-8382-4990-b612-aaaaaaaaaaaa"" Name=""Windows Azure MSDN - Visual Studio Ultimate"" /><Subscription Id=""aaaaaaaa-7aac-492c-8b08-aaaaaaaaaaaa"" Name=""Test 1"" /><Subscription Id=""aaaaaaaa-e135-45ca-bc31-aaaaaaaaaaaa"" Name=""Test 2"" /><Subscription Id=""aaaaaaaa-f3f5-42d0-ba03-aaaaaaaaaaaa"" Name=""Test 3"" /></PublishProfile></PublishData>";

		private const string V2 =
				@"<?xml version=""1.0"" encoding=""utf-8""?><PublishData><PublishProfile SchemaVersion=""2.0"" PublishMethod=""AzureServiceManagementAPI""><Subscription ServiceManagementUrl=""https://management.core.windows.net"" Id=""2ee96392-e3d2-4fcd-8e3a-d1c907c9c443"" Name=""3-Month Free Trial"" ManagementCertificate=""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"" /></PublishProfile></PublishData>";
		private const string Filename = "elastacloud.publishsettings";

		private string _fullFileName = "";

		[TestFixtureSetUp]
		public void SetUp()
		{
			var path = Path.GetTempPath();
			_fullFileName = Path.Combine(path, Filename);
			using (var writer = File.CreateText(_fullFileName))
			{
				writer.WriteLine(Publishsettings);
			}
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			if (File.Exists(_fullFileName))
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

		[Test]
		public void TestVersion2Schema()
		{
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

			var settings = PublishSettingsExtractor.GetFromXml(V2);
			var dictionary = settings.GetSubscriptions();
			dictionary.Count.Should().Be(1, "Number of subscriptions in Xml .publishsettings string");
			settings.SchemaVersion.Should().Be(2, "the schema number used");
		}

        [Test]
        public void TestVersion2SchemaWithForeginCultureSettings()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");

            var settings = PublishSettingsExtractor.GetFromXml(V2);
            var dictionary = settings.GetSubscriptions();
            dictionary.Count.Should().Be(1, "Number of subscriptions in Xml .publishsettings string");
            settings.SchemaVersion.Should().Be(2, "the schema number used");
        }
	}
}