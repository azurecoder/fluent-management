using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary> 
    /// URL encoding class.  Note: use at your own risk. 
    /// Written by: Ian Hopkins (http://www.lucidhelix.com) 
    /// Date: 2008-Dec-23 
    /// (Ported to C# by t3rse (http://www.t3rse.com)) 
    /// Update by Elastacloud (http://www.elastacloud.com) 24/04/2012
    /// </summary> 
    public static class UrlHelper
    {
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
    }
}