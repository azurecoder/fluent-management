/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Helpers.OPC
{
    public class PackageFile
    {
        private readonly string _packageFile;

        public PackageFile(string packageFile)
        {
            _packageFile = packageFile;
        }

        public bool IsOPC
        {
            get { return true; }
        }
    }
}