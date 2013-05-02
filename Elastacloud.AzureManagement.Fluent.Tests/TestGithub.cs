using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Elastacloud.AzureManagement.Fluent.Tests
{
    [TestFixture]
    public class TestGithub
    {
        const string JsonResponseCorrect = "[{\"full_name\": \"a/b\"},{\"full_name\": \"c/d\"},{\"full_name\": \"e/f\"},{\"full_name\": \"g/h\"},{\"full_name\": \"i/j\"}]";
        const string JsonResponseIncorrect = "[{\"full_name\": \"ab\"},{\"full_name\": \"c/d\"},{\"full_name\": \"e/f\"},{\"full_name\": \"g/h\"},{\"full_name\": \"i/j\"}]";
        [Test, ExpectedException(typeof(FluentManagementException), MatchType = MessageMatch.Contains, ExpectedMessage = "Invalid")]
        public void GithubClient_GetRepositories_CredentialsFail()
        {
            var helper = new Mock<IWebsiteRequestHelper>();
            //helper.Setup(a => a.GetStringResponse(null, null, null))

            var client = new GithubClient(helper.Object);

            client.GetRepositories();
        }

        [Test, ExpectedException(typeof(FluentManagementException), MatchType = MessageMatch.Contains, ExpectedMessage = "unable to parse")]
        public void GithubClient_GetRepositories_JsonFormatFail()
        {
            var helper = new Mock<IWebsiteRequestHelper>();
            helper.Setup(a => a.GetStringResponse("azurecoder", "adghjsueu374", "blah"))
                  .Returns(JsonResponseIncorrect);
            var client = new GithubClient(helper.Object)
                             {
                                 Username = "azurecoder",
                                 Password = "adghjsueu374"
                             };
            var list = client.GetRepositories("blah");
        }

        [Test]
        public void GithubClient_GetRepositories_List()
        {
            var helper = new Mock<IWebsiteRequestHelper>();
            helper.Setup(a => a.GetStringResponse("azurecoder", "adghjsueu374", "blah"))
                  .Returns(JsonResponseCorrect);
            var client = new GithubClient(helper.Object)
            {
                Username = "azurecoder",
                Password = "adghjsueu374"
            };
            var list = client.GetRepositories("blah");
            list.Count.Should().Be(5);
            list["b"].Should().Be("a");
        }
    }
}
