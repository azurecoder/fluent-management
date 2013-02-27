using System.Text.RegularExpressions;
using Elastacloud.AzureManagement.Fluent.Helpers;
using NUnit.Framework;

namespace Elastacloud.AzureManagement.Fluent.Tests
{
    [TestFixture]
    public class TestHttpUrlParse
    {
        [Test]
        public void TestUrl()
        {
            const string url = "http://elastastorage.blob.core.windows.net/vm-images/imkzmoeb.yfo20130120221739.vhd";

            var helper = new UrlHelper(url);

            Assert.AreEqual("elastastorage.blob.core.windows.net", helper.HostFullDomain);
            Assert.AreEqual("vm-images", helper.Path);
            Assert.AreEqual("imkzmoeb.yfo20130120221739.vhd", helper.File);
            Assert.AreEqual("elastastorage", helper.HostSubDomain);
        }
    }
}
