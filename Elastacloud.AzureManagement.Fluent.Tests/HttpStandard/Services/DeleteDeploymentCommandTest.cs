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
    public class DeleteDeploymentCommandTest
    {
        private Mock<IQueryManager> _mockQueryManager;
        private ServiceCommand _command;

        [SetUp]
        public void Initialize()
        {
            _mockQueryManager = new Mock<IQueryManager>();
            ServiceCommand.CurrentQueryManager = _mockQueryManager.Object;
            _command = new DeleteDeploymentCommand("bob", DeploymentSlot.Staging);
        }

        [Test]
        public void DeleteDeploymentCommand_HttpVerb_Delete()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));

            _command.SitAndWait.Set();
            _command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => tRequest.HttpVerb == "DELETE"), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        /* // https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot> */
        [Test]
        public void DeleteDeploymentCommand_Contains_Correct_Uri()
        {

            const string commandUri = "https://management.core.windows.net/aaaaaaaa-8130-49d7-95f9-aaaaaaaaaaaa/services/hostedservices/bob/deploymentslots/staging";
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
