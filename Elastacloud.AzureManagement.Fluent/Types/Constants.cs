/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Helpers;

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

        public const string PluginsRemoteAccessEnabled = "Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled";

        public const string PluginsRemoteAccessAccountUsername =
            "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername";

        public const string PluginsRemoteAccessAccountEncryptedPassword =
            "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword";

        public const string PluginsRemoteAccessAccountExpiration =
            "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration";

        public const string PluginsRemoteForwarderEnabled = "Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled";

        #region Mobile Services
        // the mobile services constants
        public const string MobileServicesSchemaVersion = "2012-05.1.0";
        public const string MobileServicesSchemaLocation = LocationConstants.WestUS;
        public const string MobileServicesVersion = "2012-05-21.1.0";
        public const string MobileServicesName = "ZumoMobileService";

        // the mobile services type templates
        public const string MobileServicesType = "Microsoft.WindowsAzure.MobileServices.MobileService";
        public const string MobileServicesSqlServerType = "Microsoft.WindowsAzure.SQLAzure.Server";
        public const string MobileServicesSqlDatabaseType = "Microsoft.WindowsAzure.SQLAzure.DataBase";

        // for settings commands
        public const string MobileServicesAuthSettings = "authsettings";
        public const string MobileServicesApnsSettings = "apns/settings";
        public const string MobileServicesLiveSettings = "livesettings";
        public const string MobileServicesServiceSettings = "settings";

        // settings for mobile services
        public const string GoogleProvider = "google";
        public const string FacebookProvider = "facebook";
        public const string TwitterProvider = "twitter";
        public const string DynamicSchemaEnabled = "dynamicSchemaEnabled";
        
        // the mobile services create template
        public const string MobileServicesCreateNewTemplate = "{{" +
                       "\"SchemaVersion\": \"{0}\", " +
                       "\"Location\": \"{1}\", " +
                       "\"ExternalResources\": {{}}, " +
                       "\"InternalResources\": {{ " +
                       "\"ZumoMobileService\": {{ " +
                       "\"ProvisioningParameters\": {{ " +
                       "\"Name\": \"{4}\", " +
                       "\"Location\": \"{5}\" " +
                       "}}, " +
                       "\"ProvisioningConfigParameters\": {{ " +
                       "\"Server\": {{ " +
                       "\"StringConcat\": [ " +
                       "{{ " +
                       "\"ResourceReference\": \"{6}.Name\" " +
                       " }}, " +
                       "\".database.windows.net\"" +
                       "] " +
                       "}}, " +
                       "\"Database\": {{ " +
                       "\"ResourceReference\": \"{7}.Name\"" +
                       "}}, " +
                       "\"AdministratorLogin\": \"{8}\", " +
                       "\"AdministratorLoginPassword\": \"{9}\" " +
                       "}}, " +
                       "\"Version\": \"{10}\", " +
                       "\"Name\": \"{11}\", " +
                       "\"Type\": \"{12}\" }}, " +
                       "\"{6}\": {{ " +
                       "\"ProvisioningParameters\": {{ " +
                       "\"AdministratorLogin\": \"{8}\", " +
                       "\"AdministratorLoginPassword\": \"{9}\", " +
                       "\"Location\": \"{5}\" " +
                       "}}, " +
                       "\"ProvisioningConfigParameters\": {{ " +
                       "\"FirewallRules\": [ {{" +
                       "\"Name\": \"AllowAllWindowsAzureIps\", " +
                       "\"StartIPAddress\": \"0.0.0.0\", " +
                       "\"EndIPAddress\": \"0.0.0.0\" }}]}}," +
                       "\"Version\": \"1.0\", " +
                       "\"Name\": \"{6}\", " +
                       "\"Type\": \"Microsoft.WindowsAzure.SQLAzure.Server\" }}," +
                       "\"{7}\": {{ " +
                       "\"ProvisioningParameters\": {{ " +
                       "\"Name\": \"{4}_db\", " +
                       "\"Edition\": \"WEB\", " +
                       "\"MaxSizeInGB\": \"1\", " +
                       "\"DBServer\": {{ " +
                       "\"ResourceReference\": \"{6}.Name\" }}," +
                       "\"CollationName\": \"SQL_Latin1_General_CP1_CI_AS\" }}," +
                       "\"Version\": \"1.0\"," +
                       "\"Name\": \"{7}\", " +
                       "\"Type\": \"Microsoft.WindowsAzure.SQLAzure.DataBase\" }}" +
                       "}} ";

        #endregion
    }
}