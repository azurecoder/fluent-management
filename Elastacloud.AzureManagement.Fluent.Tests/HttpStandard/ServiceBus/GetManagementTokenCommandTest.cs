using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.ServiceBus;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Moq;
using NUnit.Framework;

namespace Elastacloud.AzureManagement.Fluent.Tests.HttpStandard.ServiceBus
{
    [TestFixture]
    public class GetManagementTokenCommandTest
    {
        private Mock<IQueryManager> _mockQueryManager;

        [SetUp]
        public void Initialize()
        {   
            _mockQueryManager = new Mock<IQueryManager>();
            ServiceCommand.CurrentQueryManager = _mockQueryManager.Object;
        }

        [Test]
        public void GetManagementTokenCommand_HttpVerb_Post()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new GetManagementTokenCommand("bob", "bill");


            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => tRequest.HttpVerb == "POST"), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }

        [Test]
        public void GetManagementTokenCommand_CommandUri_ContainsNamespace()
        {
            _mockQueryManager.Setup(
                t =>
                t.MakeASyncRequest(It.IsAny<ServiceManagementRequest>(), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()));
            var command = new GetManagementTokenCommand("bobthisisanamespaces", "bill");


            command.SitAndWait.Set();
            command.Execute();

            _mockQueryManager.Verify(
                t =>
                t.MakeASyncRequest(It.Is<ServiceManagementRequest>(tRequest => tRequest.Body.Contains("bobthisisanamespaces")), It.IsAny<ServiceManager.AsyncResponseParser>(),
                                   It.IsAny<ServiceManager.AsyncResponseException>()), Times.Once());
        }
    }
}
