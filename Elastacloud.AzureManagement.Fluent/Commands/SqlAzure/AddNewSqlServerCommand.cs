/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.SqlAzure
{
    internal class AddNewSqlServerCommand : ServiceCommand
    {
        public const string SqlAzureManagementEndpoint = "https://management.database.windows.net:8443";
        public const string SqlAzureSchema = BaseParser.SqlAzureSchema;

        internal AddNewSqlServerCommand(string username, string password, string location)
        {
            AdministratorLogin = username;
            AdministratorPassword = password;
            HttpVerb = "POST";
            Location = location;
            ServiceType = "servers";
            BaseRequestUri = SqlAzureManagementEndpoint;
            AdditionalHeaders["x-ms-version"] = "1.0";

            if (!PasswordVerify(username, password))
                throw new ApplicationException(
                    "password must contain an alpha character, a number, upper and lower case characters");
        }

        public string AdministratorLogin { get; set; }
        // TODO: Necessary to check this against a set of rules where possible like Sql does 
        public string AdministratorPassword { get; set; }
        public string SqlAzureServerName { get; set; }

        protected override string CreateXmlPayload()
        {
            XNamespace ns = SqlAzureSchema;
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "Server",
                             new XElement(ns + "AdministratorLogin", AdministratorLogin),
                             new XElement(ns + "AdministratorLoginPassword", AdministratorPassword),
                             new XElement(ns + "Location", Location)));
            return doc.ToStringFullXmlDeclaration();
        }

        public bool PasswordVerify(string username, string password)
        {
            if (String.IsNullOrEmpty(password))
                return false;
            var regEx =
                new Regex(
                    @"^((?<upper>[A-Z])|(?<lower>[a-z])|(?<number>\d)|(?<alpha>[^(\d|[a-z]|[A-Z]|[\f\n\r\t\v)])){8,}$",
                    RegexOptions.Singleline);

            Match matches = regEx.Match(password);
            if ((matches.Groups["upper"].Captures.Count > 0 ? 1 : 0) +
                (matches.Groups["lower"].Captures.Count > 0 ? 1 : 0) +
                (matches.Groups["number"].Captures.Count > 0 ? 1 : 0) +
                (matches.Groups["alpha"].Captures.Count > 0 ? 1 : 0) < 3)
                return false;

            for (int index = 0; index <= username.Length - 3; index++)
            {
                if (password.Contains(username.Substring(index, 3)))
                    return false;
            }
            return true;
        }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            SqlAzureServerName = Parse(webResponse, BaseParser.AddNewSqlAzureServerParser);
            SitAndWait.Set();
        }
    }
}