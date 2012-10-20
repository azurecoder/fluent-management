/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// This class encapsulates the .csdef file 
    /// </summary>
    public class CsdefFile : ConfigurationFile
    {
        public static ConfigurationFile Instance;

        private CsdefFile(string name) : base(name)
        {
        }

        public static ConfigurationFile GetInstance(string name)
        {
            if (Instance == null)
                return (Instance = new CsdefFile(name));
            return Instance;
        }

        protected override string GetFileExtension()
        {
            return Constants.CsdefExtension;
        }
    }
}