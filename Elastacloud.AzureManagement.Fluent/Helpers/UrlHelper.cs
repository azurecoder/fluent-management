using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary> 
    /// URL encoding class.  Note: use at your own risk. 
    /// Written by: Ian Hopkins (http://www.lucidhelix.com) 
    /// Date: 2008-Dec-23 
    /// (Ported to C# by t3rse (http://www.t3rse.com)) 
    /// Update by Elastacloud (http://www.elastacloud.com) 24/04/2012
    /// </summary> 
    public class UrlHelper
    {
        private readonly string _url;

        public static string Encode(string str)
        {
            string charClass = String.Format("0-9a-zA-Z{0}", Regex.Escape("-_.!~*'()"));
            return Regex.Replace(str, String.Format("[^{0}]", charClass),
                                 match =>
                                 (match.Value == " ") ? "+" : String.Format("%{0:X2}", Convert.ToInt32(match.Value[0])));
        }

        public static string Decode(string str)
        {
            return Regex.Replace(str.Replace('+', ' '), "%[0-9a-zA-Z][0-9a-zA-Z]",
                                 match =>
                                 Convert.ToChar(int.Parse(match.Value.Substring(1), NumberStyles.HexNumber)).ToString(
                                     CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Used to construct a Url helper class
        /// </summary>
        /// <param name="url">The url to deconstruct</param>
        public UrlHelper(string url)
        {
            _url = url;
            Parse();
        }

        private void Parse()
        {
            const string http = "http://";
            bool containsHttp = _url.StartsWith(http, true, new CultureInfo("en-GB"));
            const string https = "https://";
            bool containsHttps = _url.StartsWith(https, true, new CultureInfo("en-GB"));

            if(!(containsHttp || containsHttps))
                throw new FluentManagementException("unknown url", "UrlHelper");

            int pos = 0;
            if (containsHttp)
                pos = _url.IndexOf(http, StringComparison.Ordinal) + http.Length;
            if(containsHttps)
                pos = _url.IndexOf(https, StringComparison.Ordinal) + https.Length;

            string query = _url.Substring(pos);

            var reg = new Regex("(?<host>.*?)/(?<path>.*?)/(?<file>.*)");
            var matches = reg.Matches(query);
            if(matches.Count != 1 && matches[0].Groups.Count != 3)
                throw new FluentManagementException("bad format for string", "UrlHelper");

            HostFullDomain = matches[0].Groups["host"].Captures[0].Value;
            Path = matches[0].Groups["path"].Captures[0].Value;
            File = matches[0].Groups["file"].Captures[0].Value;

            if(!HostFullDomain.Contains("."))
                throw new FluentManagementException("bad format for domain", "UrlHelper");

            HostSubDomain = HostFullDomain.Split('.')[0];
        }

        /// <summary>
        /// Contains the https://{1}.x.y part 
        /// </summary>
        public string HostSubDomain { get; private set; }
        /// <summary>
        /// Contains the https://{1}/x/y
        /// </summary>
        public string HostFullDomain { get; private set; }
        /// <summary>
        /// Contains the https://www.example.com/{1}/y part
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Contains the https://www.example.com/x/{1} part
        /// </summary>
        public string File { get; set; }
    }
}