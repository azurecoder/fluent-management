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
    public class CreateDeploymentCommandTest
    {
        private Mock<IQueryManager> _mockQueryManager;

        [SetUp]
        public void Initialize()
        {
            _mockQueryManager = new Mock<IQueryManager>();
            ServiceCommand.CurrentQueryManager = _mockQueryManager.Object;
        }

        [Test]
        public void CreateDeploymentCommand_HttpVerb_Post()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new CreateDeploymentCommand("bob", "bill", "mypackage", "config", DeploymentSlot.Production);

            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => tRequest.HttpVerb == "POST"), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        /*
         * XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "CreateDeployment",
                             new XElement(ns + "Name", Name),
                             new XElement(ns + "PackageUrl", PackageUri),
                             new XElement(ns + "Label", Convert.ToBase64String(Encoding.UTF8.GetBytes(DeploymentName))),
                             new XElement(ns + "Configuration", Config),
                             new XElement(ns + "StartDeployment", StartDeploymentAutomatically.ToString().ToLower()),
                             new XElement(ns + "TreatWarningsAsError", TreatWarningsAsErrors.ToString().ToLower())));
            return doc.ToStringFullXmlDeclaration();*/

        [Test]
        public void CreateDeploymentCommand_Contains_Body_Payload_PackageUrl()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new CreateDeploymentCommand("bob", "bill", "mypackage", "config", DeploymentSlot.Production);


            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "PackageUrl", "mypackage")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        [Test]
        public void CreateDeploymentCommand_Contains_Body_Payload_StartDeployment_Default_Value()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new CreateDeploymentCommand("bob", "bill", "mypackage", "config", DeploymentSlot.Production);


            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "StartDeployment", "true")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        [Test]
        public void CreateDeploymentCommand_Contains_Body_Payload_Name()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new CreateDeploymentCommand("bob", "bill", "mypackage", "config", DeploymentSlot.Production);


            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => new XmlTestHelper(tRequest.Body).CheckXmlValue(Namespaces.NsWindowsAzure, "Name", "bob")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        /* POST https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot-name> */
 
        [Test]
        public void CreateDeploymentCommand_Contains_Correct_Uri()
        {
            string commandUri =
                "https://management.core.windows.net/aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa/services/hostedservices/bob/deploymentslots/production";
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new CreateDeploymentCommand("bob", "bill", "mypackage", "config", DeploymentSlot.Production)
                {
                    SubscriptionId = "aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa"
                };


            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => command.RequestUri.ToString() == commandUri),
                    It.IsAny<ServiceManager.AsyncResponseParser>(),
                    It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }
    }
}
