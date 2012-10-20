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
    public class Constants
    {
        /// <summary>
        /// The path to the msbuild executable - uses framework v4.0.30319 
        /// TODO: will probably get it to search for a path pattern on 4
        /// </summary>
        public const string MsBuildExe = @"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";

        public const string PackageLocation = @"bin\{build}\app.publish\";
        public const string CsdefFilename = "ServiceDefinition.csdef";
        public const string CscfgFilename = "ServiceConfiguration.Cloud.cscfg";
        public const string DefaultBlobContainerName = "elastadeploy";
        public const string CscfgExtension = ".cscfg";
        public const string CcprojExtension = ".ccproj";
        public const string CsdefExtension = ".csdef";
        public const string CspkgExtension = ".cspkg";
        public const string LocationNorthEurope = "North Europe";
        public const string LocationWestEurope = "West Europe";

        public const string PluginsRemoteAccessEnabled = "Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled";

        public const string PluginsRemoteAccessAccountUsername =
            "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername";

        public const string PluginsRemoteAccessAccountEncryptedPassword =
            "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword";

        public const string PluginsRemoteAccessAccountExpiration =
            "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration";

        public const string PluginsRemoteForwarderEnabled = "Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled";
    }
}