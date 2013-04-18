using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Moq;
using NUnit.Framework;

namespace Elastacloud.AzureManagement.Fluent.Tests.HttpStandard.Services
{
    [TestFixture]
    public class AddServiceCertificateCommandTest
    {
        private Mock<IQueryManager> _mockQueryManager;
        private ServiceCommand _command;

        [SetUp]
        public void Initialize()
        {
            _mockQueryManager = new Mock<IQueryManager>();
            ServiceCommand.CurrentQueryManager = _mockQueryManager.Object;
            _command = new AddServiceCertificateCommand(new byte[] { 0x00, 0x02 }, "password", "myservice");
        }

        [Test]
        public void AddServiceCertificateCommand_HttpVerb_Post()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => tRequest.HttpVerb == "POST"), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        /*
         * XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "CertificateFile",
                             new XElement(ns + "Data", Base64CertificateData),
                             new XElement(ns + "CertificateFormat", "pfx"),
                             new XElement(ns + "Password", PfxPassword)));*/

        [Test]
        public void AddServiceCertificateCommand_Contains_Body_Payload_CertificateFormat()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "CertificateFormat", "pfx")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        [Test]
        public void AddServiceCertificateCommand_Contains_Body_Payload_Password()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "Password", "password")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        /* POST https://management.core.windows.net/<subscription-id>/certificates/ */
 
        [Test]
        public void AddServiceCertificateCommand_Contains_CorrectUri()
        {
            string commandUri =
                "https://management.core.windows.net/aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa/services/hostedservices/myservice/certificates";
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            _command.SubscriptionId = "aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa";
            _command.SitAndWait.Set();
            _command.Execute();

            Debug.WriteLine(_command.RequestUri.ToString());
            Debug.WriteLine(commandUri);

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(_ => _command.RequestUri.ToString() == commandUri),
                    It.IsAny<ServiceManager.AsyncResponseParser>(),
                    It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }
    }
}
