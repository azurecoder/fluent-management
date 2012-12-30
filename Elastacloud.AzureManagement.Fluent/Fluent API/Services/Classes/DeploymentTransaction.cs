/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Commands.Blobs;
using Elastacloud.AzureManagement.Fluent.Commands.Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    /// <summary>
    /// An IServiceTransaction implementation for deployments and hosted services
    /// </summary>
    public class DeploymentTransaction : IServiceTransaction
    {
        /// <summary>
        /// The blob client used to upload blobs and create containers
        /// </summary>
        private readonly BlobClient _blobClient;
        /// <summary>
        /// The DeploymentManager field 
        /// </summary>
        private readonly DeploymentManager _manager;

        /// <summary>
        /// Variables to hold the success and start flags
        /// </summary>
        private bool _started, _success;

        /// <summary>
        /// Constructs a DeploymenTransaction class instance
        /// </summary>
        /// <param name="manager">The DeploymentManager class which has the transaction state and context necessary</param>
        public DeploymentTransaction(DeploymentManager manager)
        {
            _manager = manager;
            _blobClient = new BlobClient(_manager.SubscriptionId, Constants.DefaultBlobContainerName, _manager.StorageAccountName, _manager.StorageAccountKey);
        }

        #region Implementation of IServiceTransaction

        /// <summary>
        /// Used to commit the transaction data 
        /// </summary>
        /// <returns>A dynamic type which represents the return of the particular transaction</returns>
        public dynamic Commit()
        {
            _started = true;
            try
            {
                // Set the config if we need to use a build step
                BuildAndPrebuildSteps();
                // upload the package to windows azure storage if the endpoint is not known
                string packageLocation = _manager.CspkgEndpoint ?? UploadPackageBlob();
                // create the deployment
                CreateDeployment(packageLocation);
                // finish the last build step to put back all of the files as necessary
                BackoutBuildStep();
            }
            catch (Exception exception)
            {
                _manager.WriteComplete(EventPoint.ExceptionOccurrence, exception.GetType() + ": " + exception.Message);
                return (_success = false);
            }
            return (_success = true);
        }

        /// <summary>
        /// Used to rollback the transaction in the event of failure 
        /// </summary>
        public void Rollback()
        {
            try
            {
                // delete the deployment 
                DeleteDeployment();
                // if the hosted service was created as part of this step then delete that
                if (!_manager.UseExistingHostedService)
                    DeleteHostedService();
                // reverse an build step that took place on the confg
                BackoutBuildStep();
            }
            catch (Exception ex)
            {
                _manager.WriteComplete(EventPoint.ExceptionOccurrence, ex.GetType() + ": " + ex.Message + ", Failed to rollback hosted service");
            }
        }

        /// <summary>
        /// Used to denote whether the transaction has been started or not
        /// </summary>
        public bool Started
        {
            get { return _started; }
        }

        /// <summary>
        /// Used to denote whether the transaction has succeeded or not
        /// </summary>
        public bool Succeeded
        {
            get { return _success; }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a deplopment based on the specific criteria specified in the DeploymentManager
        /// </summary>
        /// <param name="packageLocation">Where the package is left</param>
        private void CreateDeployment(string packageLocation)
        {
            bool deleted = false;

            if(!_blobClient.CheckStorageAccountHasResolved())
                throw new ApplicationException("unable to proceed storage account cannot be resolved after default 5 minute timeout");

            if (!_manager.UseExistingHostedService)
            {
                var hostedServiceCreate = new CreateCloudServiceCommand(_manager.HostedServiceName, _manager.Description ?? "Deployed by Fluent Management", _manager.Location)
                                              {
                                                  Certificate = _manager.ManagementCertificate,
                                                  SubscriptionId = _manager.SubscriptionId
                                              };
                hostedServiceCreate.Execute();
                _manager.WriteComplete(EventPoint.HostedServiceCreated, "Hosted service with name " + _manager.HostedServiceName + " created");
            }

            // send up service certificate - whatever happens we want the certificate up there - sometimes we may get a request but not need to alter the config of the SSL
            if (_manager.EnableSsl)
            {
                byte[] export = _manager.ServiceCertificate.Certificate.Export(X509ContentType.Pkcs12, _manager.ServiceCertificate.PvkPassword);

                var addCertificate =
                    new AddServiceCertificateCommand(export, _manager.ServiceCertificate.PvkPassword, _manager.HostedServiceName)
                        {
                            SubscriptionId = _manager.SubscriptionId,
                            Certificate = _manager.ManagementCertificate
                        };
                addCertificate.Execute();
            }

            // read in the enum value for the additional params 
            bool startImmediately = true, treatErrorsAsWarnings = false;
            if (_manager.DeploymentParams.HasValue)
            {
                startImmediately = (_manager.DeploymentParams.Value & DeploymentParams.StartImmediately) == DeploymentParams.StartImmediately;
                treatErrorsAsWarnings = (_manager.DeploymentParams.Value & DeploymentParams.WarningsAsErrors) == DeploymentParams.WarningsAsErrors;
            }
            // read in the config and convert Base64
            var deployment = new CreateDeploymentCommand(_manager.HostedServiceName, _manager.DeploymentName,packageLocation,
                                                         _manager.Base64CsfgFile, _manager.DeploymentSlot, startImmediately, treatErrorsAsWarnings)
                                 {
                                     Certificate = _manager.ManagementCertificate,
                                     SubscriptionId = _manager.SubscriptionId
                                 };

            try
            {
                deployment.Execute();
                _manager.WriteComplete(EventPoint.DeploymentCreated, "Deployment " + _manager.DeploymentName + " created");
            }
            catch (Exception)
            {
                DeleteDeployment();
                deleted = true;
            }
            finally
            {
                if (deleted)
                {
                    deployment.Execute();
                    _manager.WriteComplete(EventPoint.DeploymentCreated, "Deployment " + _manager.DeploymentName + " created");
                }
                // check here and execute on a timer to see if the role are ready and running
                if (_manager.WaitUntilAllRoleInstancesAreRunning)
                {
                    var command = new GetAggregateDeploymentStatusCommand(_manager.HostedServiceName, _manager.DeploymentSlot)
                                      {
                                          Certificate = _manager.ManagementCertificate,
                                          SubscriptionId = _manager.SubscriptionId
                                      };
                    while (!command.AllDeploymentNodesRunning)
                    {
                        command.Execute();
                        // TODO: put a 5 second timer in here for now but replace with a timeout and exception method if over a certain value
                        Thread.Sleep(5000);
                    }
                }
            }
        }


        /// <summary>
        /// This creates a blob container using the default container name "elastadeploy" and uploads the package
        /// </summary>
        private string UploadPackageBlob()
        {
            _blobClient.CreatBlobContainer();
            _manager.WriteComplete(EventPoint.StorageBlobContainerCreated, "Blob container " + Constants.DefaultBlobContainerName + " created");
            // TODO: this smells really bad fix!!
            if(_manager.LocalPackagePathName == null)
            {
                var configuration = new DeploymentConfigurationFileActivity(_manager);
                ((IDeploymentConfigurationFileActivity) configuration).WithPackageConfigDirectory(_manager.BuildActivity.PackageNameLocation);
            }
            var packageName = _manager.LocalPackagePathName;
            string deploymentPath = _blobClient.CreateAndUploadBlob(Path.GetFileName(packageName), packageName);
            
            _manager.WriteComplete(EventPoint.DeploymentPackageUploadComplete, "Uploaded package to default blob container");
            //return blobCreate.DeploymentPath;
            return deploymentPath;
        }

        /// <summary>
        /// Deletes a deployment to make space for a new one!
        /// </summary>
        private void DeleteDeployment()
        {
            // most likely there is something in the slot ...
            var deleteDeployment = new DeleteDeploymentCommand(_manager.HostedServiceName, _manager.DeploymentSlot)
                                       {
                                           Certificate = _manager.ManagementCertificate,
                                           SubscriptionId = _manager.SubscriptionId
                                       };
            deleteDeployment.Execute();
        }

        /// <summary>
        /// Deletes a deployment to make space for a new one!
        /// </summary>
        private void DeleteHostedService()
        {
            // most likely there is something in the slot ...
            var deleteHostedService = new DeleteHostedServiceCommand(_manager.HostedServiceName)
                                          {
                                              Certificate = _manager.ManagementCertificate,
                                              SubscriptionId = _manager.SubscriptionId
                                          };
            deleteHostedService.Execute();
        }

        /// <summary>
        /// Build and prebuild steps for the deployment manager when things need to be added
        /// </summary>
        private void BuildAndPrebuildSteps()
        {
            IRoleReference roleReference = new RoleReference(_manager);
            _manager.CloudConfigChanges.Add((ICloudConfig) roleReference);
            foreach (ICloudConfig item in _manager.CloudConfigChanges)
            {
                // check to see whether they are populated depending on the path we get here we may get a null reference exception otherwise
                // in only some cases will there be a need to parse both of these files sometimes we may not want to depending on whether 
                // we only want to apply settings updates to the cscfg
                if (_manager.CscfgFileInstance != null)
                    _manager.CscfgFileInstance.NewVersion = item.ChangeConfig(_manager.CscfgFileInstance.NewVersion);
                if (_manager.CsdefFileInstance != null)
                    _manager.CsdefFileInstance.NewVersion = item.ChangeDefinition(_manager.CsdefFileInstance.NewVersion);
            }
            _manager.Base64CsfgFile = _manager.ReturnBase64AmmendedString();
            _manager.BuildActivity.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BackoutBuildStep()
        {
            // if we don't call this now the package will be left in an inconsistent state with the config files
            _manager.BuildActivity.StartMsBuildProcess();
        }

        #endregion
    }
}