using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent.Tests.HttpStandard
{
    public class XmlTestHelper
    {
        private readonly XDocument _document;

        public XmlTestHelper(string body)
        {
            _document = XDocument.Parse(body);
        }

        public bool CheckXmlValue(XNamespace @namespace, string name, string value)
        {
            var checkValue = _document.Descendants(@namespace + name).FirstOrDefault().Value;
            return checkValue == value;
        }
    }
}
