using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Moq;
using NUnit.Framework;

namespace Elastacloud.AzureManagement.Fluent.Tests.HttpStandard.Services
{
    [TestFixture]
    public class CreateHostedServiceCommandTest
    {
        private Mock<IQueryManager> _mockQueryManager;
        private ServiceCommand _command;

        [SetUp]
        public void Initialize()
        {
            _mockQueryManager = new Mock<IQueryManager>();
            ServiceCommand.CurrentQueryManager = _mockQueryManager.Object;
            _command = new CreateCloudServiceCommand("bob", "bill");
        }

        [Test]
        public void CreateHostedServiceCommand_HttpVerb_Post()
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
         * <?xml version="1.0" encoding="utf-8"?>
                <CreateHostedService xmlns="http://schemas.microsoft.com/windowsazure">
                    <ServiceName>service-name</ServiceName>
                    <Label>base64-encoded-service-label</Label>
                    <Description>description</Description>
                    <Location>location</Location>
                    <AffinityGroup>affinity-group</AffinityGroup>
                </CreateHostedService>*/

        [Test]
        public void CreateHostedServiceCommand_Contains_Body_Payload_Name()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "ServiceName", "bob")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        [Test]
        public void CreateHostedServiceCommand_Contains_Body_Payload_Description()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "Description", "bill")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        [Test]
        public void CreateHostedServiceCommand_Contains_Body_Payload_Location()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
								t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "Location", LocationConstants.NorthEurope)), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        /* https://management.core.windows.net/<subscription-id>/services/hostedservices */
        [Test]
        public void CreateHostedServiceCommand_Contains_Correct_Uri()
        {
            const string commandUri = "https://management.core.windows.net/aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa/services/hostedservices";
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            _command.SubscriptionId = "aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa";

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => _command.RequestUri.ToString() == commandUri),
                    It.IsAny<ServiceManager.AsyncResponseParser>(),
                    It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }
    }
}
