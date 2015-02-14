/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Services.Classes;
using Elastacloud.AzureManagement.Fluent.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// The deployment manager will allow a hosted service and a deployment to be done 
    /// </summary>
    public class DeploymentManager : IAzureManager, IDeploymentActivity, IDeploymentConfigurationFileActivity,
                                     IDeploymentConfigurationStorageActivity, IDeploymentConfigurationParamActivity,
                                     IRoleReference, IRoleActivity, IHostedServiceActivity, ICertificateActivity,
                                     IBuildActivity, IServiceCertificate, IRemoteDesktop, IDefinitionActivity, IQueryCloudService
    {
        #region properties 

        /// <summary>
        /// The .csdef and .cscfg files are wrapped up in the ConfigurationFile classes
        /// </summary>
        public ConfigurationFile CscfgFileInstance, CsdefFileInstance;

        #region Service Certificate properties

        internal bool EnableSsl { get; set; }
        internal bool EnableRemoteDesktop { get; set; }
        internal string SslRoleName { get; set; }
        internal string RdRoleName { get; set; }
        internal string RdUsername { get; set; }
        internal string RdPassword { get; set; }

        #endregion

        public ServiceCertificate ServiceCertificate { get; private set; }
        public string HostedServiceName { get; private set; }
        public bool UseExistingHostedService { get; private set; }
        public Dictionary<string, int> RolesInstances { get; private set; }
        public DeploymentParams? DeploymentParams { get; private set; }
        public string DeploymentFolder { get; internal set; }
        public string StorageConnectionString { get; internal set; }
        public string LocalPackagePathName { get; internal set; }
        public string StorageAccountName { get; internal set; }
        public string StorageAccountKey { get; internal set; }
        public string DeploymentName { get; private set; }
        public DeploymentSlot DeploymentSlot { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public string Base64CsfgFile { get; internal set; }
        public bool WaitUntilAllRoleInstancesAreRunning { get; set; }
        /// <summary>
        /// Sets a flag to determine whether the storage account needs to have a post step to check whether the account exists
        /// </summary>
        internal bool PostStorageStep { get; set; }

        public List<ICloudConfig> CloudConfigChanges { get; private set; }
        public BuildActivity BuildActivity { get; set; }
        public string CspkgEndpoint { get; set; }

        #endregion

        internal DeploymentManager(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
            RolesInstances = new Dictionary<string, int>();
            CloudConfigChanges = new List<ICloudConfig>();
            EnableSsl = EnableRemoteDesktop = false;
        }

        #region Implementation of IDefinitionActivity

        /// <summary>
        /// Enables SSL for the particular role by updating the necessary config
        /// </summary>
        IServiceCertificate IDefinitionActivity.EnableSslForRole(string name)
        {
            EnableSsl = true;
            SslRoleName = name;
            return this;
        }

        /// <summary>
        /// Enables RD for the particular role by updating the config
        /// </summary>
        IRemoteDesktop IDefinitionActivity.EnableRemoteDesktopForRole(string name)
        {
            EnableRemoteDesktop = true;
            RdRoleName = name;
            return this;
        }

        /// <summary>
        /// Used to enable remote desktop 
        /// </summary>
        IRemoteDesktop IDefinitionActivity.EnableRemoteDesktopAndSslForRole(string name)
        {
            EnableSsl = true;
            SslRoleName = name;
            return ((IDefinitionActivity) this).EnableRemoteDesktopForRole(name);
        }

        #endregion

        #region Implementation of ICertificateActivity


        /// <summary>
        /// Adds a management certificate to the request
        /// </summary>
        IDeploymentActivity ICertificateActivity.AddCertificate(X509Certificate2 certificate)
        {
            ManagementCertificate = certificate;
            return this;
        }

        /// <summary>
        /// Given a publish settings files - adds a management certificate to the request
        /// </summary>
        IDeploymentActivity ICertificateActivity.AddPublishSettingsFromFile(string path)
        {
            var settings = new PublishSettingsExtractor(path);
            ManagementCertificate = settings.AddPublishSettingsToPersonalMachineStore();
            return this;
        }

        /// <summary>
        /// Given a publish settings block of Xml - adds a management certificate to the request
        /// </summary>
        IDeploymentActivity ICertificateActivity.AddPublishSettingsFromXml(string xml)
        {
            ManagementCertificate = PublishSettingsExtractor.GetCertificateFromXml(xml);
            return this;
        }

        /// <summary>
        /// Given a certificate thumbprint scours several local stores to find the cert by thumbprint
        /// </summary>
        IDeploymentActivity ICertificateActivity.AddCertificateFromStore(string thumbprint)
        {
            ManagementCertificate = PublishSettingsExtractor.FromStore(thumbprint);
            return this;
        }

        #endregion

        #region Implementation of IServiceCertificate

        /// <summary>
        /// Generates a service certificate and adds the appropriate text to the <certificates/> tag in the .cscfg file
        /// </summary>
        IHostedServiceActivity IServiceCertificate.GenerateAndAddServiceCertificate(string name)
        {
            ServiceCertificate = new ServiceCertificate(name);
            // we have to also create the certificate!
            ServiceCertificate.Create();

            // add these to the config list we have to implement changes to 
            if (EnableSsl)
                CloudConfigChanges.Add(new SslEnablement(ServiceCertificate, SslRoleName));
            if (EnableRemoteDesktop)
                CloudConfigChanges.Add(new RemoteDesktop(ServiceCertificate, RdRoleName)
                                           {
                                               Username = RdUsername,
                                               Password = RdPassword
                                           });

            return this;
        }

        /// <summary>
        /// Given a thumbprint and password will search the local stores to find a certificate and associated private key
        /// </summary>
        IHostedServiceActivity IServiceCertificate.UploadExistingServiceCertificate(string thumbprint, string password)
        {
            ServiceCertificate = new ServiceCertificate(string.Empty);
            ServiceCertificate.GetExisting(thumbprint, password);

            return this;
        }

        /// <summary>
        /// Uses the existing service certificate to do things
        /// Currently just a passthru - this will be implemented in future versions but it is assumed that you can only use 
        /// a single certificate with the deployment however will be use in future with a roles collection 
        /// </summary>
        IHostedServiceActivity IServiceCertificate.UsePreviouslyUploadedServiceCertificate(string name, string thumbprint)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDeploymentActivity

        /// <summary>
        /// Sets the deployment name for a new deployment
        /// </summary>
        IBuildActivity IDeploymentActivity.ForNewDeployment(string name)
        {
            DeploymentName = name;
            return this;
        }

        /// <summary>
        /// Just used for a service information query
        /// </summary>
        IQueryCloudService IDeploymentActivity.ForServiceInformationQuery()
        {
            return this;
        }

        #endregion

        #region Implementation of IDeploymentConfigurationFileActivity

        IDeploymentConfigurationStorageActivity IDeploymentConfigurationFileActivity.WithPackageConfigDirectory(string directoryName)
        {
            IDeploymentConfigurationFileActivity deploymentConfigurationActivity =
                new DeploymentConfigurationFileActivity(this);
            return deploymentConfigurationActivity.WithPackageConfigDirectory(directoryName);
        }

        #endregion

        #region Implementation of IDeploymentConfigurationStorageActivity

        IDeploymentConfigurationParamActivity IDeploymentConfigurationStorageActivity.WithStorageConnectionStringName(
            string connectionStringName)
        {
            ((IDeploymentConfigurationFileActivity) this).WithPackageConfigDirectory(BuildActivity.PackageNameLocation);
            XElement configurationSettings =
                CscfgFileInstance.NewVersion.Descendants(Namespaces.NsServiceManagement + "ServiceConfiguration")
                    .Descendants(Namespaces.NsServiceManagement + "Role")
                    .FirstOrDefault()
                    .Descendants(Namespaces.NsServiceManagement + "ConfigurationSettings")
                    .FirstOrDefault();
            string connectionString = configurationSettings.Elements(Namespaces.NsServiceManagement + "Setting")
                .Where(a => (string) a.Attribute("name") == connectionStringName)
                .Select(b => (string) b.Attribute("value"))
                .FirstOrDefault();
            if (connectionString == null)
                throw new ApplicationException("unable to find connection string");
            string[] parts = connectionString.Split(';');
            const string accountName = "AccountName=", accountKey = "AccountKey=";
            if (parts.Count() != 3 && !(parts[1].StartsWith(accountName) || parts[2].StartsWith(accountName)))
                throw new ApplicationException("connection string is not in correct format");
            StorageAccountName = parts[1].Remove(0, accountName.Count());
            StorageAccountKey = parts[2].Remove(0, accountKey.Count());
            PostStorageStep = false;
            
            return this;
        }

        IDeploymentConfigurationParamActivity IDeploymentConfigurationStorageActivity.WithStorageAccount(string storageAccountName)
        {
            PostStorageStep = true;
            StorageAccountName = storageAccountName;
            return this;
        }

       

        #endregion

        #region Implementation of IDeploymentConfigurationParamActivity

        /// <summary>
        /// Adds the params to start the service automatically and report on errors
        /// </summary>
        /// <param name="pParams">StartDeployment and TreatErrorsAsWarnings</param>
        /// <returns>An IRoleReference interface - the DeploymentManager class</returns>
        IRoleReference IDeploymentConfigurationParamActivity.AddParams(DeploymentParams? pParams)
        {
            DeploymentParams = pParams;
            return this;
        }

        /// <summary>
        /// Sets the deployment slot for the deployment in productino or staging
        /// </summary>
        /// <param name="slot">DeploymentSlot.Production or DeploymentSlot.Staging</param>
        /// <returns>An IDeploymentConfigurationParamActivity interface - the DeploymentManager class</returns>
        IDeploymentConfigurationParamActivity IDeploymentConfigurationParamActivity.AddEnvironment(DeploymentSlot slot)
        {
            DeploymentSlot = slot;
            return this;
        }

        IDeploymentConfigurationParamActivity IDeploymentConfigurationParamActivity.AddDescription(string description)
        {
            Description = description;
            return this;
        }

        IDeploymentConfigurationParamActivity IDeploymentConfigurationParamActivity.AddLocation(string location)
        {
            Location = location;
            return this;
        }

        /// <summary>
        /// Used to create a hosted service only without an attached deployment
        /// </summary>
        void IDeploymentConfigurationParamActivity.GoHostedServiceDeployment()
        {
            var create = new HostedServiceActivity(this);
            create.Create();
        }

        #endregion

        #region Implementation of IRoleReference

        /// <summary>
        /// Defines which role a particular operation should be done on
        /// </summary>
        IRoleActivity IRoleReference.ForRole(string name)
        {
            IRoleReference roleReference = new RoleReference(this);
            return roleReference.ForRole(name);
        }

        /// <summary>
        /// Defines additional roles that the operation should be done on
        /// </summary>
        IRoleActivity IRoleReference.AndRole(string name)
        {
            IRoleReference roleReference = new RoleReference(this);
            return roleReference.AndRole(name);
        }

        /// <summary>
        /// Replaces a configuration with a new .cscfg configuration 
        /// </summary>
        IRoleReference IRoleReference.ReplaceConfiguration(string filename)
        {
            IRoleReference roleReference = new RoleReference(this);
            return roleReference.ReplaceConfiguration(filename);
        }

        /// <summary>
        /// Waits until all of the role instances are running in a deployment before releasing
        /// </summary>
        /// <returns></returns>
        IServiceCompleteActivity IRoleReference.WaitUntilAllRoleInstancesAreRunning()
        {
            IRoleReference roleReference = new RoleReference(this);
            return roleReference.WaitUntilAllRoleInstancesAreRunning();
        }

        IServiceCompleteActivity IRoleReference.ReturnWithoutWaitingForRunningRoles()
        {
            IRoleReference roleReference = new RoleReference(this);
            return roleReference.ReturnWithoutWaitingForRunningRoles();
        }

        /// <summary>
        /// Goes through an effective script to determine what the user wants to do - this will involve setting up 
        /// a new hosted service or using an existing one and possibly modifying config file as well as going through a build step
        /// </summary>
        IServiceTransaction IServiceCompleteActivity.Go()
        {
            return new DeploymentTransaction(this);
        }

        #endregion

        #region Implementation of IRoleActivity

        /// <summary>
        /// Changes the instance count defined in the .cscfg file
        /// </summary>
        IRoleReference IRoleActivity.WithInstanceCount(int count)
        {
            string roleName = RolesInstances.ElementAt(RolesInstances.Count - 1).Key;
            RolesInstances[roleName] = count;

            return this;
        }

        #endregion

        #region Implementation of IHostedServiceActivity

        /// <summary>
        /// Will use an existing hosted service to deploy the deployment to 
        /// </summary>
        IDeploymentConfigurationStorageActivity IHostedServiceActivity.WithExistingHostedService(string name)
        {
            if (DeploymentName == null)
                throw new ApplicationException("Deployment name cannot be null");

            UseExistingHostedService = true;
            HostedServiceName = name;

            return this;
        }

        /// <summary>
        /// Will create a new hosted service to deploy the deployment to 
        /// </summary>
        IDeploymentConfigurationStorageActivity IHostedServiceActivity.WithNewHostedService(string name)
        {
            if (DeploymentName == null)
                throw new ApplicationException("Deployment name cannot be null");

            UseExistingHostedService = false;
            HostedServiceName = name;

            return this;
        }

       

        /// <summary>
        /// Deletes an existing hosted service and any deployments associated with it 
        /// </summary>
        /// <param name="name">the name of the hosted service</param>
        /// <returns>An IServiceCompleteActivity interface</returns>
        void IHostedServiceActivity.DeleteExistingHostedService(string name)
        {
            HostedServiceName = name;
            ActionType = ActionType.Delete;
            var action = new HostedServiceActivity(this);

            action.Delete();
        }

        #endregion

        #region Implementation of IQueryCloudService

        /// <summary>
        /// Gets a list of hosted services within a particualr subscription
        /// </summary>
        /// <returns>A List<CloudService> collection</CloudService></returns>
        List<CloudService> IQueryCloudService.GetHostedServiceList()
        {
            // build the hosted service list command here
            var command = new GetHostedServiceListCommand
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.HostedServices;
        }

        /// <summary>
        /// Gets a list of cloud services with their attached deployments
        /// </summary>
        List<CloudService> IQueryCloudService.GetCloudServiceListWithDeployments()
        {
            var cloudServices = ((IQueryCloudService)this).GetHostedServiceList();
            foreach (var cloudService in cloudServices)
            {
                var command = new GetCloudServicePropertiesCommand(cloudService.Name)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                command.Execute();
                cloudService.Deployments = command.CloudServiceDeployments;
            }
            return cloudServices;
        }

        /// <summary>
        /// Gets a list of role names for a particular hosted service in the production slot
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>A string list</returns>
        List<string> IQueryCloudService.GetRoleNamesForProductionDeploymentForServiceWithName(string serviceName)
        {
            // build the hosted service list command here
            var command = new GetDeploymenRoleNamesCommand(serviceName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.RoleNames;
        }

        /// <summary>
        /// Gets the deployment configuration in the production slot for a cloud service
        /// </summary>
        /// <param name="serviceName">The name of the cloud service</param>
        /// <returns>A CscfgFile instance</returns>
        CscfgFile IQueryCloudService.GetConfigurationForProductionDeploymentForServiceWithName(string serviceName)
        {
            // build the hosted service list command here
            var command = new GetDeploymenConfigurationCommand(serviceName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.Configuration;
        }

        /// <summary>
        /// Gets a list of hosted services that contain production deployments for the subscription
        /// </summary>
        /// <returns>A list of hosted services</returns>
        List<CloudService> IQueryCloudService.GetHostedServiceListContainingProductionDeployments()
        {
            // create a new service list 
            var services = new List<CloudService>();
            // build the hosted service list command here
            var command = new GetHostedServiceListCommand
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // enumerate the collection to see whether any of the hosted services 
            command.HostedServices.ForEach(a =>
            {
                var serviceCommand = new GetHostedServiceContainsDeploymentCommand(a.Name)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                serviceCommand.Execute();
                if (serviceCommand.ContainsProductionDeployment)
                {
                    services.Add(a);
                }
            });
            // return the new collection instead
            return services;
        }

        #endregion

        #region Implementation of IAzureManager

        public event EventReached AzureTaskComplete;
        public string SubscriptionId { get; set; }
        public X509Certificate2 ManagementCertificate { get; set; }

        #endregion

        /// <summary>
        /// Contains the type of action being performed on the service
        /// </summary>
        public ActionType ActionType { get; set; }

        internal void WriteComplete(EventPoint point, string message)
        {
            if (AzureTaskComplete != null)
                AzureTaskComplete(point, message);
        }

        /// <summary>
        /// Returns a Base64 encoded string - of the .cscfg
        /// </summary>
        internal string ReturnBase64AmmendedString()
        {
            string xml = CscfgFileInstance.NewVersion.ToStringFullXmlDeclaration();
            xml = xml.Replace('\r', ' ').Replace('\n', ' ');
            byte[] utfConfigData = Encoding.UTF8.GetBytes(xml);
            return Convert.ToBase64String(utfConfigData);
        }

        #region Implementation of IBuildActivity


        /// <summary>
        /// Sets the endpoint for the package instead of doing an upload
        /// </summary>
        IHostedServiceActivity IBuildActivity.SetCspkgEndpoint(string uriEndpoint, string cscfgFilePath)
        {
            if((uriEndpoint.StartsWith("http://") || uriEndpoint.StartsWith("https://")) && cscfgFilePath == null)
                throw new ApplicationException("please define a valid .cscfg file");
            if (BuildActivity == null)
                BuildActivity = new BuildActivity(this);
            ((IBuildActivity) BuildActivity).SetCspkgEndpoint(uriEndpoint, cscfgFilePath);

            return this;
        }

        public IHostedServiceActivity SetCspkgEndpoint(Uri uriEndpoint, XDocument configuration)
        {
            if (BuildActivity == null)
                BuildActivity = new BuildActivity(this);
            ((IBuildActivity)BuildActivity).SetCspkgEndpoint(uriEndpoint, configuration);
            return this;
        }

        /// <summary>
        /// Sets the root path to .ccproj 
        /// </summary>
        IDefinitionActivity IBuildActivity.SetBuildDirectoryRoot(string directoryName)
        {
            if (BuildActivity == null)
                BuildActivity = new BuildActivity(this);
            ((IBuildActivity) BuildActivity).SetBuildDirectoryRoot(directoryName);
            BuildActivity.Rebuild();

            return this;
        }

        #endregion

        #region Implementation of IRemoteDesktop

        /// <summary>
        /// Sets the username and password for the remote desktop configuration 
        /// </summary>
        IServiceCertificate IRemoteDesktop.WithUsernameAndPassword(string username, string password)
        {
            RdUsername = username;
            RdPassword = password;

            return this;
        }

        #endregion
    }
}