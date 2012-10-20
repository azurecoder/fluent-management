/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Linq;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary>
    /// A class used to create a random name 
    /// </summary>
    public class RandomAccountName
    {
        /// <summary>
        /// Gets a name given an init string - name can be anything 
        /// </summary>
        /// <param name="initString">Init string is a prefix for the name</param>
        /// <returns>A string name value</returns>
        public string GetNameFromInitString(string initString)
        {
            return GetName(initString);
        }

        /// <summary>
        /// Gets a pure random value for the name which will be 14 characters long
        /// </summary>
        /// <returns>A string name value</returns>
        public string GetPureRandomValue()
        {
            return GetName();
        }

        /// <summary>
        /// Gets a random name by iterating through random numbers and letters
        /// </summary>
        /// <param name="name">An init name or an empty string</param>
        /// <returns>A string name value</returns>
        private static string GetName(string name = "")
        {
            // fix the size to 9 characters
            if (name.Length > 9)
                throw new ApplicationException("unable to have a start string of more than 14 characters");
            int lengthLeftOver = 9 - name.Length;
            int seed = 0;
            var builder = new StringBuilder();
            builder.Append(name);
            string charsToChoose = "abcdefghijklmnopqrstuvwxyz0123456789";
            if (name == "")
            {
                charsToChoose = "abcdefghijklmnopqrstuvwxyz";
                seed = DateTime.Now.Millisecond;
            }
            else
            {
                seed = GenerateSeed(name);
            }
            
            var random = new Random(seed);
            for (int i = 0; i < lengthLeftOver; i++)
            {
                builder.Append(charsToChoose[(int) Math.Floor(random.NextDouble()*charsToChoose.Length)]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Generates a prefix int that can be used as a seed for different prefix values
        /// </summary>
        /// <param name="prefix">A prefix string</param>
        /// <returns>An int value</returns>
        private static int GenerateSeed(string prefix)
        {
            return prefix.Sum(letter => (int) letter);
        }
    }
}