using System;
using System.Threading.Tasks;
using CommandLine;
using Dotyk.Store.Deployment;
using Dotyk.Store.Deployment.Installer;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public class InstallOption : CommonOptions
    {
        public string PackagePath { get; set; }

        protected override async Task ExecuteOverride(ILogger logger)
        {
            using (logger.BeginScope("ExecuteInstall"))
            {
                try
                {
                    var installerFactory = new PackageInstallerFactory();
                    var appManager = new ApplicationManager();

                    var package = new InstallationPackage(PackagePath);

                    var installer = installerFactory.GetInstaller(package.Manifest.Deploy.Platform);

                    var result = await installer.InstallPackage(package, logger);

                    appManager.WriteAppPackageInstalled(
                        package.Manifest.AppId, 
                        result, 
                        writeDotykTechFields: true);
                }
                catch (Exception ex)
                {
                    logger.LogError(new EventId(), ex, "Failed to isntall package");
                    throw;
                }
            }
        }
    }
}
