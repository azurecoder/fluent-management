/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    /// <summary>
    /// Implements IBuildActivity and also builds the project through msbuild if it exists
    /// </summary>
    public class BuildActivity : IBuildActivity
    {
        private const string PublishPath = @"\app.publish";
        private const string DebugPackageRoot = @"\bin\Debug\";
        private const string ReleasePackageRoot = @"\bin\Release\";

        private readonly DeploymentManager _manager;

        public string DirectoryRoot;

        /// <summary>
        /// Sets the deployment manager and assumes that there will be a rebuild 
        /// </summary>
        public BuildActivity(DeploymentManager manager)
        {
            UseExistingBuild = false;
            _manager = manager;
        }

        #region Implementation of IBuildActivity

        /// <summary>
        /// This will set the endpoint to the .cspkg file and determine whether it exists locally or in blob storage
        /// </summary>
        IBuildActivity IBuildActivity.SetCspkgEndpoint(string uriEndpoint)
        {
            UseExistingBuild = true;
            // ensure that the string **looks** like a blob endpoint
            if (uriEndpoint.StartsWith("http") && uriEndpoint.Contains("blob"))
            {
                _manager.CspkgEndpoint = uriEndpoint;
            }
            else
            {
                if(!(uriEndpoint.Contains(Path.Combine(DebugPackageRoot, PublishPath)) || 
                    uriEndpoint.Contains(Path.Combine(ReleasePackageRoot, PublishPath))))
                {
                    throw new ApplicationException("unknown endpoint use the default azure package build path");                          
                }
                var activity = new DeploymentConfigurationFileActivity(_manager);
                ((IDeploymentConfigurationFileActivity)activity).WithPackageConfigDirectory(uriEndpoint);
            }
            
            return this;
        }

        /// <summary>
        /// Creates a buildactivity if one does not already exist and sets the ccproj file for msbuild
        /// </summary>
        IDefinitionActivity IBuildActivity.SetBuildDirectoryRoot(string directoryName)
        {
            UseExistingBuild = false;
            if (!Directory.Exists(directoryName))
                throw new ApplicationException("provided build and package root does not exist!");
            DirectoryRoot = directoryName;
            // create the two instances of the files we need to use to manipulate the content
            _manager.CscfgFileInstance = CscfgFile.GetInstance(Path.Combine(DirectoryRoot, Constants.CscfgFilename));
            _manager.CsdefFileInstance = CsdefFile.GetInstance(Path.Combine(DirectoryRoot, Constants.CsdefFilename));
            return _manager;
        }

        /// <summary>
        /// Rebuilds the build with the new params and file changes for csdef
        /// </summary>
        public void Rebuild()
        {
            int fileCount = 0;

            foreach (string fileName in Directory.EnumerateFiles(DirectoryRoot))
            {
                if (Path.GetFileName(fileName) == Constants.CsdefFilename)
                {
                    //persist the configuration file to .old
                    _manager.CsdefFileInstance.PersistConfigurationFile(ConfigurationFileType.Backup);
                    _manager.CscfgFileInstance.PersistConfigurationFile(ConfigurationFileType.Backup);
                    fileCount++;
                }
                if (Path.GetExtension(fileName) == Constants.CcprojExtension)
                {
                    RebuildWithCcProjFile(fileName);
                    fileCount++;
                }
            }
            if (fileCount != 2 || _manager.CsdefFileInstance == null || CcprojFile == null)
                throw new ApplicationException(
                    "inconsistent build files in directory, check for multiple or zero service definitions or .ccproj files");

            UseExistingBuild = false;
        }

        #endregion

        /// <summary>
        /// The ccproj file contains the necessary details for the msbuild targets to work
        /// </summary>
        public string CcprojFile { get; set; }

        /// <summary>
        /// Skip the msbuild step and use the existing build 
        /// </summary>
        public bool UseExistingBuild { get; set; }

        /// <summary>
        /// Returns the path to the package location and can be used to derive .cspkg and .cscfg
        /// </summary>
        public string PackageNameLocation
        {
            get
            {
                // An easier way to do this would be to parse the .ccproj file 
                // TODO: parse the ccproj file in a later version of the lib (v0.3.4) - both directories could exist here!!
                string pathToPackage = Path.Combine(DirectoryRoot, Constants.PackageLocation.Replace("{build}", "Debug"));
                if (!Directory.Exists(pathToPackage))
                    pathToPackage = Path.Combine(DirectoryRoot, Constants.PackageLocation.Replace("{build}", "Release"));
                if (!Directory.Exists(pathToPackage))
                    throw new ApplicationException("the output package does not exist");
                return pathToPackage;
            }
        }

        /// <summary>
        /// Build using the msbuild profile 
        /// </summary>
        public void Build()
        {
            if (UseExistingBuild)
                return;
            // TODO:We need to do the same for the ServiceConfiguration.cscfg drop this into another method
            _manager.CsdefFileInstance.PersistConfigurationFile(ConfigurationFileType.Current);
            _manager.CscfgFileInstance.PersistConfigurationFile(ConfigurationFileType.Current);
            // calls the start msbuild process
            Process msbuildProcess = null;
            try
            {
                msbuildProcess = StartMsBuildProcess();
            }
            catch (Exception e)
            {
                Trace.WriteLine("unable to complete MsBuild process returned with error: " + e.Message);
            }
            finally
            {
                // delete the old backup file and use this to replace the existing file .csdef file
                _manager.CsdefFileInstance.RollbackConfigurationFile();
                _manager.CscfgFileInstance.RollbackConfigurationFile();
            }

            if (msbuildProcess.ExitCode > 0)
            {
                throw new ApplicationException("build failed!");
            }
        }

        /// <summary>
        /// Creates a buildactivity if one does not already exist and sets the ccproj file for msbuild
        /// </summary>
        private void RebuildWithCcProjFile(string filePath)
        {
            if (Path.GetExtension(filePath) != Constants.CcprojExtension)
                throw new ApplicationException("not a .ccproj file!");

            CcprojFile = filePath;
        }

        /// <summary>
        /// Defines the msbuild process to be used to build the cloud project
        /// </summary>
        internal Process StartMsBuildProcess()
        {
            if (UseExistingBuild)
                return null;
            //invoke msbuild here!
            string pathToMsBuild = Environment.ExpandEnvironmentVariables(Constants.MsBuildExe);
            var info = new ProcessStartInfo(pathToMsBuild, "\"" + CcprojFile + "\" /target:publish")
                           {
                               UseShellExecute = false,
                               RedirectStandardOutput = true
                           };

            var msbuildProcess = new Process
                                     {
                                         StartInfo = info
                                     };
            bool started = msbuildProcess.Start();
            // TODO: Make a proper detail on the exception information by pulling back any FAILED messages or ERRORS from the output stream
            string output = msbuildProcess.StandardOutput.ReadToEnd();
            if (!started)
                throw new ApplicationException("unable to start msbuild - do you have .NET framework v4 installed?");
            msbuildProcess.WaitForExit();
            return msbuildProcess;
        }
    }
}