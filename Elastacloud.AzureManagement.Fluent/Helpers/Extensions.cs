/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    public static class Extensions
    {
        public static string ToStringFullXmlDeclaration(this XDocument doc)
        {
            var builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                doc.Save(writer);
            }
            return builder.ToString().Replace("utf-16", "utf-8");
        }

        /// <summary>
        /// Removes all of the \r and \n charaters
        /// </summary>
        /// <param name="doc">The Xml document</param>
        /// <returns>A string value containing XML</returns>
        public static string ToStringFullXmlDeclarationWithReplace(this XDocument doc)
        {
            var builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                doc.Save(writer);
            }
            string withUtf16 = builder.ToString().Replace("utf-16", "utf-8");
            return withUtf16.Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}